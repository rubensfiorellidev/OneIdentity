using OneID.Domain.Entities.AlertsContext;

namespace OneID.Application.Interfaces.Repositories
{
    public interface IAlertSettingsRepository
    {
        Task<AlertSettings> GetAsync(CancellationToken ct = default);
        Task UpdateAsync(AlertSettings updated, CancellationToken ct = default);
    }

}
