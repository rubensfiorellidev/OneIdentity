using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Npgsql;
using OneID.Application.Interfaces.Repositories;
using OneID.Application.Interfaces.Services;
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
        public static IServiceCollection AddData(this IServiceCollection services, IConfiguration configuration, IHostEnvironment environment)
        {
            services.AddDbContextInfra(configuration, environment);

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
            services.AddScoped<IAccessPackageQueryRepository, AccessPackageQueryRepository>();
            services.AddScoped<IUserClaimWriterRepository, UserClaimWriterRepository>();


            return services;
        }

        public static IServiceCollection AddDbContextInfra(this IServiceCollection services, IConfiguration configuration, IHostEnvironment environment)
        {
            services.AddDbContextFactory<OneDbContext>((serviceProvider, options) =>
            {
                string connectionString = environment.IsDevelopment()
                    ? configuration.GetConnectionString("NPSqlConnection")
                    : environment.IsEnvironment("Staging")
                        ? configuration.GetConnectionString("NPSqlConnectionStaging")
                        : configuration.GetConnectionString("NPSqlConnectionProduction");

                var dataSourceBuilder = new NpgsqlDataSourceBuilder(connectionString);
                var dataSource = dataSourceBuilder.Build();

                options.UseNpgsql(dataSource, npgsqlOptions =>
                {
                    npgsqlOptions.MigrationsAssembly(typeof(OneDbContext).Assembly.FullName);
                    npgsqlOptions.EnableRetryOnFailure(5, TimeSpan.FromSeconds(30), null);
                });

                options.EnableSensitiveDataLogging(environment.IsDevelopment());

            });



            services.AddScoped<IOneDbContextFactory, OneDbContextFactory>();

            return services;
        }
        #endregion

    }

}
