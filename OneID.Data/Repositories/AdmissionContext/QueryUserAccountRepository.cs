using Microsoft.EntityFrameworkCore;
using OneID.Application.Interfaces.Repositories;
using OneID.Data.Factories;
using OneID.Domain.Entities.UserContext;

namespace OneID.Data.Repositories.AdmissionContext
{
    public class QueryUserAccountRepository : IQueryUserAccountRepository
    {
        private readonly OneDbContextFactory _dbContextFactory;

        public QueryUserAccountRepository(OneDbContextFactory dbContextFactory)
        {
            _dbContextFactory = dbContextFactory;
        }

        public async Task<UserAccount?> GetByCorrelationIdAsync(Guid correlationId, CancellationToken cancellationToken = default)
        {
            await using var dbContext = _dbContextFactory.CreateDbContext();

            return await dbContext.UserAccounts
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.CorrelationId == correlationId, cancellationToken);
        }

        public async Task<UserAccount?> GetByCpfHashAsync(string cpfHash, CancellationToken cancellationToken = default)
        {
            await using var dbContext = _dbContextFactory.CreateDbContext();

            return await dbContext.UserAccounts
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.CpfHash == cpfHash, cancellationToken);
        }
    }

}
