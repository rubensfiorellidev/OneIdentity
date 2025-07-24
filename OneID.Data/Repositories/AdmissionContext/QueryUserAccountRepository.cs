using Microsoft.EntityFrameworkCore;
using OneID.Application.DTOs.Admission;
using OneID.Application.Interfaces.Repositories;
using OneID.Data.Interfaces;
using OneID.Domain.Entities.UserContext;

#nullable disable
namespace OneID.Data.Repositories.AdmissionContext
{
    public class QueryUserAccountRepository : IQueryUserAccountRepository
    {
        private readonly IOneDbContextFactory _dbContextFactory;

        public QueryUserAccountRepository(IOneDbContextFactory dbContextFactory)
        {
            _dbContextFactory = dbContextFactory;
        }

        public async Task<UserAccount> GetByCorrelationIdAsync(Guid correlationId, CancellationToken cancellationToken = default)
        {
            await using var dbContext = _dbContextFactory.CreateDbContext();

            return await dbContext.UserAccounts
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.CorrelationId == correlationId, cancellationToken);
        }

        public async Task<UserAccount> GetByCpfHashAsync(string cpfHash, CancellationToken cancellationToken = default)
        {
            await using var dbContext = _dbContextFactory.CreateDbContext();

            return await dbContext.UserAccounts
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.CpfHash == cpfHash, cancellationToken);
        }

        public async Task<List<RecentAdmissionDto>> GetRecentAdmissionsAsync(int limit, CancellationToken cancellationToken = default)
        {
            await using var dbContext = _dbContextFactory.CreateDbContext();

            return await dbContext.UserAccounts
                .OrderByDescending(x => x.ProvisioningAt)
                .Take(limit)
                .AsNoTracking()
                .Select(x => new RecentAdmissionDto
                {
                    AccountId = x.Id,
                    FullName = x.FullName,
                    DepartmentName = x.DepartmentName,
                    JobTitleName = x.JobTitleName,
                    ProvisioningAt = x.ProvisioningAt
                })
                .ToListAsync(cancellationToken);

        }
    }

}
