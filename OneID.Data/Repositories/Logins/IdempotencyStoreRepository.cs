using OneID.Application.Interfaces.Logins;
using OneID.Data.Interfaces;
using OneID.Domain.Entities.Logins;

namespace OneID.Data.Repositories.Logins
{
    public sealed class IdempotencyStoreRepository : IIdempotencyStoreRepository
    {
        private readonly IOneDbContextFactory _dbContextFactory;

        public IdempotencyStoreRepository(IOneDbContextFactory dbContextFactory)
        {
            _dbContextFactory = dbContextFactory;
        }

        public async Task<(bool found, string? payload)> TryGetAsync(string key, CancellationToken ct = default)
        {
            using var _db = _dbContextFactory.CreateDbContext();

            var row = await _db.Set<IdempotencyEntry>().FindAsync([key], ct);
            if (row != null)
                return (row is not null, row?.Payload);

            return (false, null);
        }

        public async Task MarkAsync(string key, object payload, CancellationToken ct = default)
        {
            using var _db = _dbContextFactory.CreateDbContext();

            var json = System.Text.Json.JsonSerializer.Serialize(payload);
            _db.Set<IdempotencyEntry>().Add(
                new IdempotencyEntry(key,
                                     json
            ));

            await _db.SaveChangesAsync(ct);
        }
    }

}
