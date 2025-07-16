using OneID.Domain.Entities.AlertsContext;

namespace OneID.Application.Interfaces.Repositories
{
    public interface IAlertSettingsRepository
    {
        Task<AlertSettings?> GetAsync(CancellationToken ct = default);
        Task<bool> ExistsAsync(CancellationToken ct = default);
        Task<AlertSettings?> GetByIdAsync(string id, CancellationToken ct);
        Task AddAsync(AlertSettings entity, CancellationToken ct = default);
        Task UpdateAsync(AlertSettings entity, CancellationToken ct = default);
    }

}
