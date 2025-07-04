using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OneID.Application.Abstractions;
using OneID.Application.Interfaces;
using OneID.Application.Services.KeyCloakServices;

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
                client.BaseAddress = new Uri($"{keycloak.BaseUrl}realms/{keycloak.Realm}/protocol/openid-connect/");
            });

            services.AddHttpClient("KeycloakAdmin", client =>
            {
                var keycloak = configuration.GetSection("Keycloak").Get<KeycloakOptions>();
                client.BaseAddress = new Uri($"{keycloak.BaseUrl}");
            });

            services.AddScoped<IKeycloakTokenService, KeycloakTokenService>();
            services.AddScoped<IKeycloakUserChecker, KeycloakUserChecker>();


            return services;
        }
    }
}
