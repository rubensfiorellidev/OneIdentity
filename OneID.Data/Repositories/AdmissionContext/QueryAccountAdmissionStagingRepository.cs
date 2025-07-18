using Microsoft.EntityFrameworkCore;
using OneID.Application.Interfaces.Repositories;
using OneID.Data.Interfaces;
using OneID.Domain.Entities.UserContext;
using OneID.Domain.ValueObjects;

namespace OneID.Data.Repositories.AdmissionContext
{
    public sealed class QueryAccountAdmissionStagingRepository : IQueryAccountAdmissionStagingRepository
    {
        private readonly IOneDbContextFactory _dbContextFactory;

        public QueryAccountAdmissionStagingRepository(IOneDbContextFactory dbContextFactory)
        {
            _dbContextFactory = dbContextFactory;
        }

        public async Task<AccountAdmissionStaging?> GetByCorrelationIdAsync(Guid correlationId, CancellationToken cancellationToken = default)
        {
            await using var dbContext = _dbContextFactory.CreateDbContext();

            return await dbContext.AccountAdmissionStagings
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.CorrelationId == correlationId, cancellationToken);

        }

        public async Task<List<AccountAdmissionStaging>> GetPendingAsync(CancellationToken cancellationToken)
        {
            await using var db = _dbContextFactory.CreateDbContext();

            var result = await db.AccountAdmissionStagings
                .Where(x => x.Status == AdmissionStatus.Pending)
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync(cancellationToken);

            return result;
        }
    }
}
