using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using OneID.Application.Abstractions;
using OneID.Data.DataContexts;
using OneID.Domain.Entities.UserContext;
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

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
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
                    });

            return services;
        }
        #endregion

    }
}
