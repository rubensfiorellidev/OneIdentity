using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Npgsql;
using OneID.Application.Interfaces.Repositories;
using OneID.Application.Services.RefreshTokens;
using OneID.Data.DataContexts;
using OneID.Data.Factories;
using OneID.Data.Interfaces;
using OneID.Data.Repositories.AdmissionContext;
using OneID.Data.Repositories.DeduplicationSagaContext;
using OneID.Data.Repositories.RefreshTokens;
using OneID.Data.Repositories.StoredEvents;
using OneID.Data.Repositories.UsersContext;
using OneID.Domain.Interfaces;


#nullable disable
namespace OneID.Data
{
    public static class DependencyInjection
    {
        #region Data
        public static IServiceCollection AddData(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IOneDbContextFactory, OneDbContextFactory>();

            services.AddScoped<IAdmissionAuditRepository, AdmissionAuditRepository>();
            services.AddScoped<ILoginExistsUserRepository, LoginExistsUserRepository>();
            services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
            services.AddScoped<IRefreshTokenService, RefreshTokenService>();
            services.AddScoped<IStoredEventRepository, StoredEventRepository>();
            services.AddScoped<IAddUserAccountRepository, AddUserAccountRepository>();
            services.AddScoped<IAddUserAccountStagingRepository, AddUserAccountStagingRepository>();
            services.AddScoped<IDeduplicationKeyRepository, DeduplicationKeyRepository>();
            services.AddScoped<IDeduplicationRepository, SagaDeduplicationRepository>();
            services.AddScoped<IQueryAccountAdmissionStagingRepository, QueryAccountAdmissionStagingRepository>();
            services.AddScoped<IAlertSettingsRepository, AlertSettingsRepository>();
            services.AddScoped<IQueryUserAccountRepository, QueryUserAccountRepository>();
            services.AddScoped<IAdmissionAlertRepository, AdmissionAlertRepository>();




            services.AddDbContextFactory<OneDbContext>((serviceProvider, opts) =>
            {
                var env = serviceProvider.GetRequiredService<IHostEnvironment>();
                var config = serviceProvider.GetRequiredService<IConfiguration>();

                string connectionString = env.IsDevelopment()
                    ? config.GetConnectionString("NPSqlConnection")
                    : env.IsEnvironment("Staging")
                        ? config.GetConnectionString("NPSqlConnectionStaging")
                        : config.GetConnectionString("NPSqlConnectionProduction");

                var dataSourceBuilder = new NpgsqlDataSourceBuilder(connectionString);
                var dataSource = dataSourceBuilder.Build();

                opts.UseNpgsql(dataSource, npgsqlOptions =>
                {
                    npgsqlOptions.MigrationsAssembly(typeof(OneDbContext).Assembly.FullName);
                    npgsqlOptions.EnableRetryOnFailure(5, TimeSpan.FromSeconds(30), null);
                });

                opts.EnableSensitiveDataLogging(false);
            });

            return services;
        }
        #endregion

    }

}
