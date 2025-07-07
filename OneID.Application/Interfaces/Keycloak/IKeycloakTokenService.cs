namespace OneID.Application.Interfaces.Keycloak
{
    public interface IKeycloakTokenService
    {
        Task<string> GetAccessTokenAsync(CancellationToken ct);
    }

}
