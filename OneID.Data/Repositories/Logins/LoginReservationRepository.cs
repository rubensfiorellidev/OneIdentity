using Microsoft.EntityFrameworkCore;
using OneID.Application.Interfaces.Logins;
using OneID.Data.Interfaces;
using OneID.Domain.Entities.Logins;

namespace OneID.Data.Repositories.Logins
{
    public sealed class LoginReservationRepository : ILoginReservationRepository
    {
        private readonly IOneDbContextFactory _dbContextFactory;
        public LoginReservationRepository(IOneDbContextFactory dbContextFactory)
        {
            _dbContextFactory = dbContextFactory;
        }

        public async Task<string> ReserveAsync(string preferredLogin, Guid correlationId, CancellationToken ct)
        {
            using var _db = _dbContextFactory.CreateDbContext();
            var candidate = preferredLogin;

            for (var i = 0; i < 50; i++)
            {
                try
                {
                    _db.Set<LoginReservation>().Add(new LoginReservation
                    (
                        candidate,
                        correlationId,
                        "Reserved"
                    ));
                    await _db.SaveChangesAsync(ct);
                    return candidate;
                }
                catch (DbUpdateException ex) when (IsUniqueViolation(ex))
                {
                    candidate = ComposeCandidate(preferredLogin, i + 1);
                    _db.ChangeTracker.Clear();
                }
            }

            throw new InvalidOperationException("Não foi possível reservar um login único após várias tentativas.");
        }

        public async Task CommitAsync(Guid correlationId, CancellationToken ct)
        {
            await using var db = _dbContextFactory.CreateDbContext();

            await db.Set<LoginReservation>()
                .Where(x => x.CorrelationId == correlationId && x.Status != "Committed")
                .ExecuteUpdateAsync(s => s
                    .SetProperty(r => r.Status, "Committed")
                    .SetProperty(r => r.UpdatedAtUtc, DateTimeOffset.UtcNow),
                    ct);
        }

        public async Task ReleaseAsync(Guid correlationId, CancellationToken ct)
        {
            await using var db = _dbContextFactory.CreateDbContext();

            await db.Set<LoginReservation>()
                .Where(x => x.CorrelationId == correlationId && x.Status != "Committed" && x.Status != "Released")
                .ExecuteUpdateAsync(s => s
                    .SetProperty(r => r.Status, "Released")
                    .SetProperty(r => r.UpdatedAtUtc, DateTimeOffset.UtcNow),
                    ct);
        }

        private static bool IsUniqueViolation(DbUpdateException ex)
            => ex.InnerException is Npgsql.PostgresException pex
               && pex.SqlState == Npgsql.PostgresErrorCodes.UniqueViolation;

        private static string ComposeCandidate(string baseLogin, int counter)
        {
            var raw = $"{baseLogin}{counter}";
            return raw.Length <= 12 ? raw : raw[..12];
        }
    }
}
