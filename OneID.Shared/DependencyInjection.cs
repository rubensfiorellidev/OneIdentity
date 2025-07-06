using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using OneID.Application.Abstractions;
using OneID.Application.Services.RefreshTokens;
using OneID.Data.DataContexts;
using OneID.Domain.Entities.UserContext;
using OneID.Shared.Authentication;
using System.Security.Cryptography;

#nullable disable
namespace OneID.Shared
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration, IHostEnvironment environment)
        {

            services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssemblies(AppDomain.CurrentDomain.GetAssemblies());
                cfg.AddOpenBehavior(typeof(RequestLoggingPipelineBehavior<,>));
            });


            return services;
        }

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

            services.AddIdentity<ApplicationUser, IdentityRole>()
               .AddEntityFrameworkStores<OneDbContext>()
               .AddDefaultTokenProviders();

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
                        if (context.Request.Headers.TryGetValue("Authorization", out var authHeader))
                        {
                            var authHeaderValue = authHeader.ToString();
                            Console.WriteLine($"🟢 Authorization header recebido: {authHeaderValue}");

                            if (authHeaderValue.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
                            {
                                var token = authHeaderValue.Substring("Bearer ".Length).Trim();
                                Console.WriteLine($"🟢 Token extraído: {token}");
                            }
                            else
                            {
                                Console.WriteLine("⚠ Authorization header não contém Bearer");
                            }
                        }
                        else
                        {
                            Console.WriteLine("⚠ Nenhum Authorization header recebido");
                        }

                        return Task.CompletedTask;
                    },
                    OnAuthenticationFailed = context =>
                    {
                        Console.WriteLine($"🔴 Falha na autenticação: {context.Exception.Message}");
                        context.NoResult();
                        context.Response.StatusCode = 401;
                        context.Response.ContentType = "application/json";
                        return context.Response.WriteAsync("{\"error\":\"Token inválido ou malformado\"}");
                    },
                    OnTokenValidated = context =>
                    {
                        Console.WriteLine($"✅ Token validado com sucesso para: {context.Principal.Identity?.Name ?? "unknown"}");
                        return Task.CompletedTask;
                    },
                    OnChallenge = context =>
                    {
                        Console.WriteLine("⚠ Challenge acionado");
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


            services.TryAddScoped<IRefreshTokenService, RefreshTokenService>();
            services.TryAddScoped<JwtProvider>();
            services.ConfigureOptions<JwtOptionsSetup>();

            return services;
        }
        #endregion

    }
}
