using Microsoft.EntityFrameworkCore;
using Npgsql;
using OneID.Application.Interfaces.Repositories;
using OneID.Data.Interfaces;
using OneID.Domain.Entities.TotpOptions;

namespace OneID.Data.Repositories.Tokens
{
    public sealed class EfTotpReplayGuardRepository : ITotpReplayGuardRepository
    {
        private readonly IOneDbContextFactory _factory;
        public EfTotpReplayGuardRepository(IOneDbContextFactory factory)
            => _factory = factory;

        public async Task<bool> IsReplayAsync(string userId, long windowStartUnix, CancellationToken ct)
        {
            await using var db = _factory.CreateDbContext();

            return await db.Set<TotpCodeUseEntity>()
                           .AsNoTracking()
                           .AnyAsync(x => x.UserId == userId && x.Step == windowStartUnix, ct);
        }

        public async Task MarkUsedAsync(string userId, long windowStartUnix, CancellationToken ct)
        {
            await using var db = _factory.CreateDbContext();

            // TTL apenas para housekeeping; ajuste conforme seu período/deriva do TOTP
            var expiresAt = DateTimeOffset.UtcNow.AddMinutes(10);

            db.Set<TotpCodeUseEntity>().Add(
                new TotpCodeUseEntity(userId, windowStartUnix, expiresAt)
            );

            try
            {
                await db.SaveChangesAsync(ct);
            }
            catch (DbUpdateException ex) when (IsUniqueViolation(ex)) { }
        }

        private static bool IsUniqueViolation(DbUpdateException ex)
        => ex.InnerException is PostgresException pg
           && pg.SqlState == PostgresErrorCodes.UniqueViolation;
    }
}
