namespace OneID.Application.Interfaces.Keycloak
{
    public interface IKeycloakUserCreator
    {
        Task CreateUserAsync(
                string username,
                string password,
                string email,
                string firstName,
                string lastName,
                CancellationToken ct);
    }

}
