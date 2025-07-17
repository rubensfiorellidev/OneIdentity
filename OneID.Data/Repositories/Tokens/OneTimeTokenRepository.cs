using OneID.Application.Interfaces.Repositories;
using OneID.Data.Interfaces;
using OneID.Domain.Entities.Tokens;

namespace OneID.Data.Repositories.Tokens
{
    public class OneTimeTokenRepository : IOneTimeTokenRepository
    {
        private readonly IOneDbContextFactory _dbContextFactory;

        public OneTimeTokenRepository(IOneDbContextFactory dbContextFactory)
        {
            _dbContextFactory = dbContextFactory;
        }

        public async Task SaveAsync(string jti, string correlationId, DateTimeOffset expiresAt, CancellationToken ct)
        {
            await using var db = _dbContextFactory.CreateDbContext();

            var entity = new OneTimeToken(jti, correlationId, expiresAt);
            db.OneTimeTokens.Add(entity);
            await db.SaveChangesAsync(ct);
        }

        public async Task<bool> IsValidAsync(string jti, CancellationToken ct)
        {
            await using var db = _dbContextFactory.CreateDbContext();

            var token = await db.OneTimeTokens.FindAsync(new object[] { jti }, ct);
            return token?.IsValid() ?? false;
        }

        public async Task MarkAsUsedAsync(string jti, CancellationToken ct)
        {
            await using var db = _dbContextFactory.CreateDbContext();

            var token = await db.OneTimeTokens.FindAsync(new object[] { jti }, ct);
            if (token is null) return;

            token.MarkAsUsed();
            await db.SaveChangesAsync(ct);
        }
    }

}
