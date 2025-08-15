using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OneID.Application.Interfaces.Repositories;
using OneID.Application.Services;
using OneID.Data.DataContexts;
using OneID.Data.Factories;
using OneID.Data.Interfaces;
using StackExchange.Redis;


#nullable disable
namespace OneID.Data
{
    public static class DependencyInjection
    {
        #region Data
        public static IServiceCollection AddData(this IServiceCollection services, IConfiguration configuration, IHostEnvironment environment)
        {
            services.AddDbContextInfra(configuration, environment);

            services.AddSingleton<IConnectionMultiplexer>(_ =>
                ConnectionMultiplexer.Connect(configuration.GetConnectionString("Redis")));

            services.Scan(scan => scan
                    .FromAssembliesOf(typeof(IAdmissionAuditRepository), typeof(IOneDbContextFactory))
                    .AddClasses(classes => classes
                        .InNamespaces(
                            "OneID.Data.Repositories",
                            "OneID.Data.Repositories.UsersContext",
                            "OneID.Data.Redis"
                        )
                    .Where(t => t.Name.EndsWith("Repository")))
                    .AsImplementedInterfaces()
                    .WithScopedLifetime()
            );


            return services;
        }

        public static IServiceCollection AddDbContextInfra(this IServiceCollection services, IConfiguration configuration, IHostEnvironment environment)
        {
            services.AddScoped<IOneDbContextFactory, OneDbContextFactory>();
            services.AddSingleton<AuditStampInterceptor>();

            services.AddDbContextFactory<OneDbContext>((sp, options) =>
            {
                configuration = sp.GetRequiredService<IConfiguration>();
                environment = sp.GetRequiredService<IHostEnvironment>();

                string connectionString = environment.IsDevelopment()
                    ? configuration.GetConnectionString("NPSqlConnection")
                    : environment.IsEnvironment("Staging")
                        ? configuration.GetConnectionString("NPSqlConnectionStaging")
                        : configuration.GetConnectionString("NPSqlConnectionProduction");

                var dataSource = new Npgsql.NpgsqlDataSourceBuilder(connectionString).Build();

                options.UseNpgsql(dataSource, npgsqlOptions =>
                {
                    npgsqlOptions.MigrationsAssembly(typeof(OneDbContext).Assembly.FullName);
                    npgsqlOptions.EnableRetryOnFailure(5, TimeSpan.FromSeconds(30), null);
                });

                options.AddInterceptors(sp.GetRequiredService<AuditStampInterceptor>());

                options.EnableSensitiveDataLogging(environment.IsDevelopment());
            });

            return services;
        }
        #endregion

    }

}
