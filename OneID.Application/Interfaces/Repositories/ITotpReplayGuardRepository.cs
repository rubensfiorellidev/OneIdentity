namespace OneID.Application.Interfaces.Repositories
{
    public interface ITotpReplayGuardRepository
    {
        Task<bool> IsReplayAsync(string userId, long windowStartUnix, CancellationToken ct);
        Task MarkUsedAsync(string userId, long windowStartUnix, CancellationToken ct);
    }
}
