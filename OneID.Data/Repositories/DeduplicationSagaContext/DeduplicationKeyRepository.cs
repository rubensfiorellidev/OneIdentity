using Microsoft.EntityFrameworkCore;
using OneID.Application.Interfaces.Repositories;
using OneID.Data.Interfaces;
using OneID.Domain.Entities.Sagas;

namespace OneID.Data.Repositories.DeduplicationSagaContext
{
    public class DeduplicationKeyRepository : IDeduplicationKeyRepository
    {
        private readonly IOneDbContextFactory _dbContextFactory;

        public DeduplicationKeyRepository(IOneDbContextFactory contextFactory)
        {
            _dbContextFactory = contextFactory;
        }

        public async Task<bool> ExistsAsync(string businessKey, string processName, CancellationToken cancellationToken)
        {
            await using var db = _dbContextFactory.CreateDbContext();

            return await db.SagaDeduplicationKeys
                .AsNoTracking()
                .AnyAsync(x => x.BusinessKey == businessKey && x.ProcessName == processName, cancellationToken);

        }

        public async Task RemoveAsync(string businessKey, string processName, CancellationToken cancellationToken)
        {
            await using var db = _dbContextFactory.CreateDbContext();

            var entity = await db.SagaDeduplicationKeys
                .FirstOrDefaultAsync(x => x.BusinessKey == businessKey && x.ProcessName == processName, cancellationToken);

            if (entity is not null)
            {
                db.SagaDeduplicationKeys.Remove(entity);
                await db.SaveChangesAsync(cancellationToken);
            }
        }

        public async Task SaveAsync(string businessKey, string processName, CancellationToken cancellationToken)
        {
            await using var db = _dbContextFactory.CreateDbContext();

            db.SagaDeduplicationKeys.Add(new SagaDeduplicationKey
            {
                BusinessKey = businessKey,
                ProcessName = processName,
                CreatedAt = DateTimeOffset.UtcNow
            });

            await db.SaveChangesAsync(cancellationToken);
        }
    }
}
