namespace OneID.Application.Interfaces.Repositories
{
    public interface ISagaDeduplicationRepository
    {
        Task<bool> ExistsAsync(Guid correlationId, CancellationToken cancellationToken);
        Task SaveAsync(Guid correlationId, string processName, CancellationToken cancellationToken);
        Task RemoveAsync(Guid correlationId, CancellationToken cancellationToken);

    }
}
