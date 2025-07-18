using Amazon;
using Amazon.Extensions.NETCore.Setup;
using Amazon.Runtime;
using Amazon.SimpleEmail;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using OneID.Application.Abstractions;
using OneID.Application.Interfaces.CQRS;
using OneID.Application.Interfaces.Interceptor;
using OneID.Application.Interfaces.SES;
using OneID.Application.Services;
using OneID.Application.Services.RefreshTokens;
using OneID.Application.Services.SES;
using OneID.Domain.Contracts.Jwt;
using OneID.Domain.Interfaces;
using OneID.Shared.Authentication;
using OneID.Shared.Services;
using System.IO.Compression;
using System.Security.Cryptography;
using System.Threading.RateLimiting;

#nullable disable
namespace OneID.Shared
{
    public static class DependencyInjection
    {
        #region Infra
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration, IHostEnvironment environment)
        {
            var awsOptions = new AWSOptions
            {
                Region = RegionEndpoint.GetBySystemName(configuration["AWS:Region"]),
                Credentials = new BasicAWSCredentials(
                    configuration["AWS:AccessKey"],
                    configuration["AWS:SecretKey"]
                )
            };

            services.AddDefaultAWSOptions(awsOptions);
            services.AddAWSService<IAmazonSimpleEmailService>();
            services.Configure<SesSettings>(configuration.GetSection("SesSettings"));
            services.AddSingleton<ISesEmailSender, SesEmailSender>();
            services.AddScoped<ICurrentUserService, CurrentUserService>();

            // Command handlers
            services.Scan(scan => scan
                .FromApplicationDependencies()
                .AddClasses(classes => classes.AssignableTo(typeof(ICommandHandler<,>)))
                .AsImplementedInterfaces()
                .WithScopedLifetime());

            // Query handlers
            services.Scan(scan => scan
                .FromApplicationDependencies()
                .AddClasses(classes => classes.AssignableTo(typeof(IQueryHandler<,>)))
                .AsImplementedInterfaces()
                .WithScopedLifetime());

            // Decorators e dispatchers
            services.Decorate(typeof(ICommandHandler<,>), typeof(LoggingCommandHandlerDecorator<,>));
            services.AddScoped<ISender, Sender>();
            services.AddScoped<IQueryExecutor, QueryDispatcher>();

            return services;
        }
        #endregion

        #region Pipeline
        public static IServiceCollection AddApiPipelineConfiguration(this IServiceCollection services)
        {
            services.AddMemoryCache();

            services.AddHttpContextAccessor();

            services
               .AddResponseCompression(opts =>
               {
                   opts.Providers.Add<GzipCompressionProvider>();
                   opts.Providers.Add<BrotliCompressionProvider>();
                   opts.EnableForHttps = true;
               });

            services
                .Configure<BrotliCompressionProviderOptions>(opts =>
                {
                    opts.Level = CompressionLevel.Optimal;

                });

            services
                .Configure<GzipCompressionProviderOptions>(opts =>
                {
                    opts.Level = CompressionLevel.Optimal;

                });

            services
    .AddRateLimiter(opts =>
    {
        opts.AddSlidingWindowLimiter("sliding", opts =>
        {
            opts.PermitLimit = 1000; // Permitir até 1000 requisições por janela
            opts.Window = TimeSpan.FromSeconds(60); // Janela de 1 minuto
            opts.SegmentsPerWindow = 10; // Dividido em 10 segmentos (6 segundos cada)
            opts.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
            opts.QueueLimit = 100;

        })
        .OnRejected = async (context, token) =>
        {
            var retryAfterHeader = context.Lease != null && context.Lease.TryGetMetadata(MetadataName.RetryAfter, out var retryAfter)
                ? TimeSpan.FromSeconds(retryAfter.TotalSeconds)
                : TimeSpan.FromSeconds(25);

            context.HttpContext.Response.StatusCode = 429;
            context.HttpContext.Response.Headers.Append("Retry-After", retryAfterHeader.TotalSeconds.ToString());

            var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<object>>();
            logger.LogWarning(
                "Rate limit exceeded for {ClientIP} at {Time}.",
                context.HttpContext.Connection.RemoteIpAddress,
                DateTime.UtcNow);

            await context.HttpContext.Response.WriteAsync(
                $"Rate limit exceeded. Try again after {retryAfterHeader.TotalSeconds} seconds.",
                cancellationToken: token);
        };

    });

            services.AddCors(options =>
            {
                options.AddPolicy("AllowAll", builder =>
                {
                    builder.AllowAnyOrigin()
                           .AllowAnyMethod()
                           .AllowAnyHeader();
                });

                options.AddPolicy("OneIdBackendCustom", builder =>
                {
                    builder.SetIsOriginAllowed(origin =>
                        Uri.TryCreate(origin, UriKind.Absolute, out var uri) &&
                        uri.Host.EndsWith(".oneid.cloud", StringComparison.OrdinalIgnoreCase))
                           .AllowAnyMethod()
                           .AllowAnyHeader()
                           .AllowCredentials();
                });
            });

            return services;
        }
        #endregion

        #region JwtWebTokens
        public static IServiceCollection AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            var jwtSettings = configuration.GetSection("Jwt");

            var issuer = jwtSettings["Issuer"];
            var audience = jwtSettings["Audience"];
            var publicKeyPath = jwtSettings["PublicKeyPath"];

            var xmlKey = File.ReadAllText(publicKeyPath);
            var rsa = RSA.Create();
            rsa.FromXmlString(xmlKey);
            var rsaKey = new RsaSecurityKey(rsa);

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = issuer,
                    ValidAudience = audience,
                    IssuerSigningKey = rsaKey

                };

                options.Events = new JwtBearerEvents
                {
                    OnAuthenticationFailed = context =>
                    {
                        context.NoResult();
                        context.Response.StatusCode = 401;
                        context.Response.ContentType = "application/json";
                        return context.Response.WriteAsync("{\"error\":\"Token inválido ou malformado\"}");
                    },
                    OnChallenge = context =>
                    {
                        if (!context.Response.HasStarted)
                        {
                            context.Response.StatusCode = 401;
                            context.Response.ContentType = "application/json";
                            return context.Response.WriteAsync("{\"error\":\"Token ausente ou inválido\"}");
                        }
                        return Task.CompletedTask;
                    }
                };
            });

            services.AddAuthentication()
                .AddJwtBearer("RequestToken", options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = issuer,
                        ValidAudience = audience,
                        IssuerSigningKey = rsaKey,
                        ClockSkew = TimeSpan.FromSeconds(2)
                    };

                    options.Events = new JwtBearerEvents
                    {
                        OnAuthenticationFailed = context =>
                        {
                            context.NoResult();
                            context.Response.StatusCode = 401;
                            context.Response.ContentType = "application/json";
                            return context.Response.WriteAsync("{\"error\":\"Token de requisição inválido ou malformado\"}");
                        },
                        OnChallenge = context =>
                        {
                            if (!context.Response.HasStarted)
                            {
                                context.Response.StatusCode = 401;
                                context.Response.ContentType = "application/json";
                                return context.Response.WriteAsync("{\"error\":\"Token de requisição ausente ou inválido\"}");
                            }
                            return Task.CompletedTask;
                        }
                    };
                });


            services.TryAddScoped<IRefreshTokenService, RefreshTokenService>();
            services.TryAddScoped<JwtProvider>();
            services.ConfigureOptions<JwtOptionsSetup>();
            services.AddScoped<IJwtProvider, JwtProvider>();


            return services;
        }
        #endregion

    }
}
