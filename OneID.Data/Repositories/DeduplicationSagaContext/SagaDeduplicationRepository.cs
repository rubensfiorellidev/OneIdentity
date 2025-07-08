using Microsoft.EntityFrameworkCore;
using OneID.Application.Interfaces.Repositories;
using OneID.Data.Interfaces;
using OneID.Domain.Entities.Sagas;

namespace OneID.Data.Repositories.DeduplicationSagaContext
{
    public sealed class SagaDeduplicationRepository : ISagaDeduplicationRepository
    {
        IOneDbContextFactory _dbContextFactory;

        public SagaDeduplicationRepository(IOneDbContextFactory dbContextFactory)
        {
            _dbContextFactory = dbContextFactory;
        }

        public async Task<bool> ExistsAsync(Guid correlationId, CancellationToken cancellationToken)
        {
            await using var db = _dbContextFactory.CreateDbContext();

            return await db.SagaDeduplications
                .AsNoTracking()
                .AnyAsync(x => x.CorrelationId == correlationId, cancellationToken);
        }

        public async Task RemoveAsync(Guid correlationId, CancellationToken cancellationToken)
        {
            await using var db = _dbContextFactory.CreateDbContext();

            var entity = await db.SagaDeduplications
                .FirstOrDefaultAsync(x => x.CorrelationId == correlationId, cancellationToken);

            if (entity is not null)
            {
                db.SagaDeduplications.Remove(entity);
                await db.SaveChangesAsync(cancellationToken);
            }
        }

        public async Task SaveAsync(Guid correlationId, string processName, CancellationToken cancellationToken)
        {
            await using var db = _dbContextFactory.CreateDbContext();

            db.SagaDeduplications.Add(new SagaDeduplication
            {
                CorrelationId = correlationId,
                ProcessName = processName,
                CreatedAt = DateTimeOffset.UtcNow
            });
        }
    }
}
