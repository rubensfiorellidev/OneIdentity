using OneID.Domain.Entities.AuditSagas;

namespace OneID.Application.Interfaces.Repositories
{
    public interface IAdmissionAuditRepository
    {
        Task AddAsync(AdmissionAudit audit, CancellationToken cancellationToken);

    }
}
