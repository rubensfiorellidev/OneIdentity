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
using OneID.Application.Interfaces.Interceptor;
using OneID.Application.Interfaces.SES;
using OneID.Application.Services.RefreshTokens;
using OneID.Application.Services.SES;
using OneID.Data.Redis;
using OneID.Domain.Contracts.Jwt;
using OneID.Domain.Interfaces;
using OneID.Shared.Authentication;
using OneID.Shared.Services;
using OpenTelemetry;
using OpenTelemetry.Exporter;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using StackExchange.Redis;
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

            return services;
        }
        #endregion

        #region Pipeline
        public static IServiceCollection AddApiPipelineConfiguration(this IServiceCollection services)
        {
            services.AddMemoryCache();
            services.AddHttpClient();
            services.AddControllers();
            services.AddHttpContextAccessor();

            services
               .AddResponseCompression(opts =>
               {
                   opts.Providers.Add<GzipCompressionProvider>();
                   opts.Providers.Add<BrotliCompressionProvider>();
                   opts.EnableForHttps = true;

                   opts.MimeTypes =
                    [
                        "application/json",
                        "application/xml",
                        "text/plain",
                        "text/json",
                        "text/html",
                        "text/css",
                        "application/javascript"
                    ];
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
                    OnMessageReceived = context =>
                    {
                        context.Token = ExtractToken(context.Request);
                        return Task.CompletedTask;
                    },
                    OnTokenValidated = context =>
                    {
                        var scope = context.Principal.FindFirst("access_scope")?.Value;
                        if (scope is not ("bootstrap_token" or "token_request_only" or "user_access"))
                        {
                            context.Fail("Token não autorizado.");
                        }

                        return Task.CompletedTask;

                    },
                    OnAuthenticationFailed = context =>
                    {
                        context.NoResult();
                        context.Response.StatusCode = 401;
                        context.Response.ContentType = "application/json";
                        return context.Response.WriteAsync("{\"error\":\"Token inválido ou malformado\"}");
                    },
                    OnChallenge = async context =>
                    {
                        if (!context.Response.HasStarted)
                            return;

                        context.HandleResponse();
                        context.Response.StatusCode = 401;
                        context.Response.ContentType = "application/json";
                        await context.Response.WriteAsync("{\"error\":\"Token inválido ou malformado\"}");
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
        private static string ExtractToken(HttpRequest request)
        {
            var authHeader = request.Headers["Authorization"].ToString();
            if (!string.IsNullOrWhiteSpace(authHeader) && authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
            {
                return authHeader["Bearer ".Length..].Trim();
            }

            var cookieToken = request.Cookies["access_token"];
            return !string.IsNullOrWhiteSpace(cookieToken) ? cookieToken : null;
        }

        #endregion

        #region Redis
        public static IServiceCollection AddRedis(this IServiceCollection services, IConfiguration configuration)
        {
            var redisOptions = new RedisOptions();
            configuration.GetSection("Redis").Bind(redisOptions);

            var configRedisOptions = new ConfigurationOptions
            {
                EndPoints = { { redisOptions.EndPoint, redisOptions.Port } },
                User = redisOptions.User,
                Password = redisOptions.Password,
                Ssl = redisOptions.Ssl,
                SslProtocols = System.Security.Authentication.SslProtocols.Tls12,
                ConnectRetry = redisOptions.ConnectRetry,
                ReconnectRetryPolicy = new LinearRetry(redisOptions.ReconnectRetryDelay),
                AbortOnConnectFail = false,
                AllowAdmin = false,
                ConnectTimeout = redisOptions.ConnectTimeout,
                DefaultDatabase = 0,
                SyncTimeout = redisOptions.SyncTimeout,
                AsyncTimeout = redisOptions.AsyncTimeout,
                KeepAlive = redisOptions.KeepAlive
            };

            var connectionMultiplexer = ConnectionMultiplexer.Connect(configRedisOptions);
            services.AddSingleton<IConnectionMultiplexer>(connectionMultiplexer);

            return services;
        }
        #endregion

        #region OpenTelemetry
        public static IServiceCollection AddOpenTelemetry(this IServiceCollection services, IConfiguration configuration, IHostEnvironment environment)
        {
            var serviceName = configuration["OpenTelemetry:ServiceName"] ?? environment.ApplicationName ?? "oneid-api";
            var environmentName = configuration["OpenTelemetry:Environment"] ?? environment.EnvironmentName ?? "development";

            services.AddOpenTelemetry()
                .ConfigureResource(resource => resource
                    .AddService(serviceName)
                    .AddAttributes([
                        new KeyValuePair<string, object>("deployment.environment", environmentName),
                        new KeyValuePair<string, object>("service.namespace", configuration["OpenTelemetry:ServiceNamespace"])
                    ]))
                .WithTracing(tracer => tracer
                    .SetSampler(new AlwaysOnSampler())
                    .AddSource("oneid-auth")
                    .AddSource("oneid-auto")
                    .AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation()
                    .AddOtlpExporter(opt =>
                    {
                        opt.Endpoint = new Uri(configuration["OpenTelemetry:Endpoint"]);
                        opt.Headers = configuration["OpenTelemetry:Headers"];
                        opt.Protocol = OtlpExportProtocol.HttpProtobuf;
                        opt.ExportProcessorType = ExportProcessorType.Simple;
                    })
                );

            return services;
        }
        #endregion


    }

}