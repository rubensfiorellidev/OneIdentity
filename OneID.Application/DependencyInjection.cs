using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OneID.Application.Interfaces;
using OneID.Application.Services;
using OneID.Application.Services.KeyCloakServices;
using OneID.Domain.Entities.KeycloakOptions;

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



            return services;
        }
    }
}
