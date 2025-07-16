using Microsoft.EntityFrameworkCore;
using OneID.Application.Interfaces.Repositories;
using OneID.Data.Interfaces;
using OneID.Domain.Entities.AlertsContext;

#nullable disable
namespace OneID.Data.Repositories.AdmissionContext
{
    public class AlertSettingsRepository : IAlertSettingsRepository
    {
        private readonly IOneDbContextFactory _dbContextFactory;

        public AlertSettingsRepository(IOneDbContextFactory dbContextFactory)
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

        public async Task<bool> ExistsAsync(CancellationToken ct = default)
        {
            await using var dbContext = _dbContextFactory.CreateDbContext();

            return await dbContext.AlertSettings.AnyAsync(ct);
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

        public async Task AddAsync(AlertSettings entity, CancellationToken ct = default)
        {
            await using var dbContext = _dbContextFactory.CreateDbContext();

            await dbContext.AlertSettings.AddAsync(entity, ct);
            await dbContext.SaveChangesAsync(ct);
        }

        public async Task<AlertSettings> GetByIdAsync(string id, CancellationToken ct)
        {
            await using var dbContext = _dbContextFactory.CreateDbContext();

            return await dbContext.AlertSettings.FirstOrDefaultAsync(x => x.Id == id, ct);
        }

    }
}
