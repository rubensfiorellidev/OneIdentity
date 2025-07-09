using OneID.Application.Interfaces.Repositories;
using OneID.Data.Factories;
using OneID.Domain.Entities.UserContext;

namespace OneID.Data.Repositories.AdmissionContext
{
    public sealed class AdmissionCltAlertRepository : IAdmissionAlertRepository
    {
        private readonly OneDbContextFactory _dbContextFactory;

        public AdmissionCltAlertRepository(OneDbContextFactory dbContextFactory)
        {
            _dbContextFactory = dbContextFactory;
        }

        public async Task AddAsync(AdmissionAlert alert, CancellationToken ct)
        {
            await using var dbContext = _dbContextFactory.CreateDbContext();

            dbContext.Set<AdmissionAlert>().Add(alert);

            await dbContext.SaveChangesAsync(ct).ConfigureAwait(false);
        }
    }
}
