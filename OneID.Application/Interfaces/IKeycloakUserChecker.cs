namespace OneID.Application.Interfaces
{
    public interface IKeycloakUserChecker
    {
        Task<bool> UsernameExistsAsync(string username, CancellationToken ct);

    }
}
