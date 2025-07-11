using OneID.Domain.Entities.TokenResults;

namespace OneID.Application.Interfaces.Keycloak
{
    public interface IKeycloakAuthService
    {
        Task<KeycloakTokenResult> AuthenticateAsync(string username, string password);
    }

}
