using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OneID.Application.Interfaces.Logins;
using OneID.Application.Interfaces.Repositories;
using OneID.Application.Interfaces.Services;
using OneID.Application.Services;
using OneID.Application.Services.RefreshTokens;
using OneID.Data.DataContexts;
using OneID.Data.Factories;
using OneID.Data.Interfaces;
using OneID.Data.Redis;
using OneID.Data.Repositories.AdmissionContext;
using OneID.Data.Repositories.DeduplicationSagaContext;
using OneID.Data.Repositories.Logins;
using OneID.Data.Repositories.RefreshTokens;
using OneID.Data.Repositories.RolesContext;
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
            services.AddScoped<IUserRoleWriterRepository, UserRoleWriterRepository>();
            services.AddScoped<IRoleWriterRepository, RoleWriterRepository>();
            services.AddScoped<IRedisRepository, RedisRepository>();
            services.AddScoped<IQueryUserRepository, QueryUserRepository>();
            services.AddScoped<ILoginReservationRepository, LoginReservationRepository>();
            services.AddScoped<IIdempotencyStore, IdempotencyStore>();





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
