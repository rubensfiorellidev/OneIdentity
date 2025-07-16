using OneID.Domain.Entities.UserContext;

namespace OneID.Application.Interfaces.Repositories
{
    public interface IAdmissionAlertRepository
    {
        Task AddAsync(AdmissionAlert alert, CancellationToken ct);

    }
}
