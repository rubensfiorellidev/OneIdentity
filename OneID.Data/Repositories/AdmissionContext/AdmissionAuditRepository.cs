using OneID.Application.Interfaces;
using OneID.Domain.Entities;

namespace OneID.Data.Repositories.AdmissionContext
{
    public class AdmissionAuditRepository : IAdmissionAuditRepository
    {
        private readonly IOneDbContextFactory _dbContext;
        public AdmissionAuditRepository(IOneDbContextFactory dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task AddAsync(AdmissionAudit audit, CancellationToken cancellationToken)
        {
            await using var dbContext = _dbContext.CreateDbContext();

            await dbContext.AdmissionAudits.AddAsync(audit, cancellationToken);

            await dbContext.SaveChangesAsync(cancellationToken);
        }
    }

}
