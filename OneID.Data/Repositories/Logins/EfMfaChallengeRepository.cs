using Microsoft.EntityFrameworkCore;
using OneID.Application.Interfaces.Repositories;
using OneID.Data.Interfaces;
using OneID.Domain.Entities.Logins;

#nullable disable
namespace OneID.Data.Repositories.Logins
{
    public sealed class EfMfaChallengeRepository : IMfaChallengeRepository
    {
        private readonly IOneDbContextFactory _oneDbContextFactory;

        public EfMfaChallengeRepository(IOneDbContextFactory oneDbContextFactory)
        {
            _oneDbContextFactory = oneDbContextFactory;
        }

        public async Task<MfaChallengeEntity> CreateAsync(MfaChallengeEntity challenge, CancellationToken ct)
        {
            await using var dbContext = _oneDbContextFactory.CreateDbContext();

            dbContext.MfaChallenges.Add(challenge);
            await dbContext.SaveChangesAsync(ct);
            return challenge;
        }

        public async Task ExpireAsync(string jti, CancellationToken ct)
        {
            await using var dbContext = _oneDbContextFactory.CreateDbContext();
            var now = DateTimeOffset.UtcNow;

            await dbContext.MfaChallenges
                .Where(x => x.Jti == jti && !x.Used && x.ExpiresAt > now)
                .ExecuteUpdateAsync(setters => setters
                    .SetProperty(x => x.ExpiresAt, now),
                    ct
                );
        }

        public async Task<MfaChallengeEntity> GetByIdAsync(string jti, CancellationToken ct)
        {
            await using var dbContext = _oneDbContextFactory.CreateDbContext();

            var entity = await dbContext.MfaChallenges
                .AsNoTracking()
                .SingleOrDefaultAsync(x => x.Jti == jti, ct)
                .ConfigureAwait(false);

            return entity;
        }

        public async Task<MfaChallengeEntity> GetLatestActiveAsync(string userId, CancellationToken ct)
        {
            await using var dbContext = _oneDbContextFactory.CreateDbContext();
            var now = DateTimeOffset.UtcNow;

            return await dbContext.MfaChallenges
                .AsNoTracking()
                .Where(x => x.UserId == userId
                            && !x.Used
                            && x.ExpiresAt > now)
                .OrderByDescending(x => x.CreatedAt)
                .FirstOrDefaultAsync(ct)
                .ConfigureAwait(false);
        }

        public async Task<bool> MarkUsedAsync(string jti, bool success, CancellationToken ct)
        {
            await using var dbContext = _oneDbContextFactory.CreateDbContext();
            var now = DateTimeOffset.UtcNow;

            var affected = await dbContext.MfaChallenges
                .Where(x => x.Jti == jti && !x.Used && x.ExpiresAt > now)
                .ExecuteUpdateAsync(setters => setters
                    .SetProperty(x => x.Used, true)
                    .SetProperty(x => x.ConsumedAt, now),
                    ct
                );

            return affected > 0;
        }
    }
}
