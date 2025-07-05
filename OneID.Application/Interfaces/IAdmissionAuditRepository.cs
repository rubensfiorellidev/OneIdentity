using OneID.Domain.Entities.AuditSagas;

namespace OneID.Application.Interfaces
{
    public interface IAdmissionAuditRepository
    {
        Task AddAsync(AdmissionAudit audit, CancellationToken cancellationToken);

    }
}
