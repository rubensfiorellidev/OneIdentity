namespace OneID.Application.Interfaces.Logins
{
    public interface IIdempotencyStore
    {
        Task<(bool found, string? payload)> TryGetAsync(string key, CancellationToken ct = default);
        Task MarkAsync(string key, object payload, CancellationToken ct = default);
    }
}
