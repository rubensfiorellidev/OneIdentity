using OneID.Domain.Entities.UserContext;

namespace OneID.Application.Interfaces.Repositories
{
    public interface IQueryAccountAdmissionStagingRepository
    {
        Task<AccountAdmissionStaging?> GetByCorrelationIdAsync(Guid correlationId, CancellationToken cancellationToken = default);
        Task<List<AccountAdmissionStaging>> GetPendingAsync(CancellationToken cancellationToken);

    }

}
