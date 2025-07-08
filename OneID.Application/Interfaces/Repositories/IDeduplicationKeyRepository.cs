namespace OneID.Application.Interfaces.Repositories
{
    public interface IDeduplicationKeyRepository
    {
        Task<bool> ExistsAsync(string businessKey, string processName, CancellationToken cancellationToken);
        Task SaveAsync(string businessKey, string processName, CancellationToken cancellationToken);
        Task RemoveAsync(string businessKey, string processName, CancellationToken cancellationToken);

    }
}
