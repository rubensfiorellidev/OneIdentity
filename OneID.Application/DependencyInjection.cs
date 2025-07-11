using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OneID.Application.Builders;
using OneID.Application.Interfaces.AesCryptoService;
using OneID.Application.Interfaces.Builders;
using OneID.Application.Interfaces.Keycloak;
using OneID.Application.Interfaces.SensitiveData;
using OneID.Application.Interfaces.Services;
using OneID.Application.Services;
using OneID.Application.Services.AesCryptoServices;
using OneID.Application.Services.KeyCloakServices;
using OneID.Application.Services.StrategyEvents;
using OneID.Domain.Abstractions.EventsContext;
using OneID.Domain.Abstractions.Factories;
using OneID.Domain.Contracts;
using OneID.Domain.Entities.KeycloakOptions;
using OneID.Domain.Interfaces;

#nullable disable
namespace OneID.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<KeycloakOptions>(configuration.GetSection("Keycloak"));

            services.AddHttpClient("KeycloakClient", client =>
            {
                var keycloak = configuration.GetSection("Keycloak").Get<KeycloakOptions>();
                client.BaseAddress = new Uri($"{keycloak.BaseUrl.TrimEnd('/')}/");
            });

            services.AddHttpClient("KeycloakAdmin", client =>
            {
                var keycloak = configuration.GetSection("Keycloak").Get<KeycloakOptions>();
                client.BaseAddress = new Uri($"{keycloak.BaseUrl.TrimEnd('/')}/");
            });

            services.AddScoped<IUserLoginGenerator, UserLoginGenerator>();
            services.AddScoped<IKeycloakUserCreator, KeycloakUserCreator>();
            services.AddScoped<IKeycloakUserChecker, KeycloakUserChecker>();
            services.AddScoped<IKeycloakTokenService, KeycloakTokenService>();
            services.AddScoped<IEventStrategy, UserAccountCreatedEventStrategy>();
            services.AddScoped<IEventStrategy, UserAccountCreationFailedEventStrategy>();
            services.AddScoped<IFactoryEventStrategy, FactoryEventStrategy>();
            services.AddScoped<IEventDispatcher, EventDispatcher>();
            services.AddScoped<IUserAccountBuilder, UserAccountBuilder>();
            services.AddScoped<IUserAccountStagingBuilder, UserAccountStagingBuilder>();
            services.AddScoped<IAlertNotifier, AlertNotifier>();
            services.AddHttpClient<IKeycloakAuthService, KeycloakAuthService>();
            services.AddScoped<IAccessPackageClaimService, AccessPackageClaimService>();




            services.AddTransient<ICryptoService>(provider =>
            {
                var config = provider.GetRequiredService<IConfiguration>();
                var logger = provider.GetRequiredService<ILogger<AesCryptoService>>();

                var key = config["EncryptionSettings:Key"];
                var crypt = config["EncryptionSettings:Crypt"];

                if (string.IsNullOrWhiteSpace(key)) throw new InvalidOperationException("EncryptionSettings:Key is missing.");
                if (string.IsNullOrWhiteSpace(crypt)) throw new InvalidOperationException("EncryptionSettings:Crypt is missing.");

                return new AesCryptoService(key, crypt, logger);
            });

            services.AddTransient<IHashService, Sha3HashService>();
            services.AddTransient<ISensitiveDataEncryptionServiceUserAccount, SensitiveDataEncryptionServiceUserAccount>();



            return services;
        }
    }
}
