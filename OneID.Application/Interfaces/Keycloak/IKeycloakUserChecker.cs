namespace OneID.Application.Interfaces.Keycloak
{
    public interface IKeycloakUserChecker
    {
        Task<bool> UsernameExistsAsync(string username, CancellationToken ct);

    }
}
