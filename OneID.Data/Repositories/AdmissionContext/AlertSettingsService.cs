using Microsoft.EntityFrameworkCore;
using OneID.Application.Interfaces.Repositories;
using OneID.Data.Interfaces;
using OneID.Domain.Entities.AlertsContext;

namespace OneID.Data.Repositories.AdmissionContext
{
    public class AlertSettingsService : IAlertSettingsRepository
    {
        private readonly IOneDbContextFactory _dbContextFactory;

        public AlertSettingsService(IOneDbContextFactory dbContextFactory)
        {
            _dbContextFactory = dbContextFactory;
        }

        public async Task<AlertSettings> GetAsync(CancellationToken ct = default)
        {
            await using var dbContext = _dbContextFactory.CreateDbContext();

            var settings = await dbContext.AlertSettings.FirstOrDefaultAsync(ct);
            if (settings is null)
            {
                settings = new AlertSettings();
                dbContext.AlertSettings.Add(settings);

                await dbContext.SaveChangesAsync(ct);
            }
            return settings;
        }

        public async Task UpdateAsync(AlertSettings updated, CancellationToken ct = default)
        {
            await using var dbContext = _dbContextFactory.CreateDbContext();

            var existing = await GetAsync(ct);

            existing.CriticalRecipients = updated.CriticalRecipients;
            existing.WarningRecipients = updated.WarningRecipients;
            existing.InfoRecipients = updated.InfoRecipients;

            dbContext.AlertSettings.Update(existing);
            await dbContext.SaveChangesAsync(ct);

        }
    }
}
