using OneID.Application.Interfaces.Repositories;
using OneID.Data.Interfaces;
using OneID.Domain.Entities.UserContext;

namespace OneID.Data.Repositories.AdmissionContext
{
    public sealed class AdmissionAlertRepository : IAdmissionAlertRepository
    {
        private readonly IOneDbContextFactory _dbContextFactory;

        public AdmissionAlertRepository(IOneDbContextFactory dbContextFactory)
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
