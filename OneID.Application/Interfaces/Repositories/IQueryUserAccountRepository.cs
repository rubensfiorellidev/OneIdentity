using OneID.Domain.Entities.UserContext;

namespace OneID.Application.Interfaces.Repositories
{
    public interface IQueryUserAccountRepository
    {
        Task<UserAccount> GetByCpfHashAsync(string CpfHash, CancellationToken cancellationToken = default);
        Task<UserAccount> GetByCorrelationIdAsync(Guid correlationId, CancellationToken cancellationToken = default);
        Task<List<UserAccount>> GetRecentAdmissionsAsync(int limit, CancellationToken cancellationToken = default);
    }
}
