using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Npgsql;
using OneID.Data.DataContexts;
using OneID.Domain.Entities;
using System.Text;

#nullable disable
namespace OneID.Data
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddData(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContextFactory<OneIdDbContext>((serviceProvider, opts) =>
            {
                var env = serviceProvider.GetRequiredService<IHostEnvironment>();
                var config = serviceProvider.GetRequiredService<IConfiguration>();

                string connectionString = env.IsDevelopment()
                    ? config.GetConnectionString("NPSqlConnection")
                    : env.IsEnvironment("Staging")
                        ? config.GetConnectionString("NPSqlConnectionQa")
                        : config.GetConnectionString("NPSqlConnectionPrd");

                var dataSourceBuilder = new NpgsqlDataSourceBuilder(connectionString);
                var dataSource = dataSourceBuilder.Build();

                opts.UseNpgsql(dataSource, npgsqlOptions =>
                {
                    npgsqlOptions.MigrationsAssembly(typeof(OneIdDbContext).Assembly.FullName);
                    npgsqlOptions.EnableRetryOnFailure(5, TimeSpan.FromSeconds(30), null);
                });

                opts.EnableSensitiveDataLogging(false);
            });

            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<OneIdDbContext>()
                .AddDefaultTokenProviders();

            return services;
        }

        #region JwtWebTokens
        public static IServiceCollection AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            var jwtSettings = configuration.GetSection("Jwt");

            var issuer = jwtSettings["Issuer"];
            var audience = jwtSettings["Audience"];
            var keyPathOrValue = jwtSettings["PrivateKeyPath"];

            string key;
            if (File.Exists(keyPathOrValue))
            {
                key = File.ReadAllText(keyPathOrValue);
            }
            else
            {
                key = keyPathOrValue;
            }

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
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key))
                    };
                });

            return services;
        }
        #endregion

    }
}
