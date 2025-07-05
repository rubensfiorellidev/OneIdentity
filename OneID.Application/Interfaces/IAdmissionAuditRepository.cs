using OneID.Domain.Entities;

namespace OneID.Application.Interfaces
{
    public interface IAdmissionAuditRepository
    {
        Task AddAsync(AdmissionAudit audit, CancellationToken cancellationToken);

    }
}
