namespace OneID.Application.Interfaces.Repositories
{
    public interface IOneTimeTokenRepository
    {
        Task SaveAsync(string jti, string correlationId, DateTimeOffset expiresAt, CancellationToken ct);
        Task<bool> IsValidAsync(string jti, CancellationToken ct);
        Task MarkAsUsedAsync(string jti, CancellationToken ct);
    }

}
