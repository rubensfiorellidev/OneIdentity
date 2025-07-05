namespace OneID.Application.Interfaces
{
    public interface IKeycloakTokenService
    {
        Task<string> GetAccessTokenAsync(CancellationToken ct);
    }

}
