using Microsoft.EntityFrameworkCore;
using OneID.Application.Interfaces.Repositories;
using OneID.Data.Interfaces;
using OneID.Domain.Entities.UserContext;

namespace OneID.Data.Repositories.AdmissionContext
{
    public class QueryAccountAdmissionStagingRepository : IQueryAccountAdmissionStagingRepository
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
    }
}
