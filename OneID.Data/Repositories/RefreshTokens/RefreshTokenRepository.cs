using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.DependencyInjection;
using OneID.Application.Interfaces.Repositories;
using OneID.Data.DataContexts;
using OneID.Domain.Entities.JwtWebTokens;

namespace OneID.Data.Repositories.RefreshTokens
{
    public sealed class RefreshTokenRepository : IRefreshTokenRepository
    {
        private readonly IServiceProvider _serviceProvider;
        public RefreshTokenRepository(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        private OneDbContext DbContext =>
            _serviceProvider.GetRequiredService<OneDbContext>();

        public async Task AddAsync(RefreshWebToken token)
        {

            await DbContext.RefreshWebTokens.AddAsync(token);
            await DbContext.SaveChangesAsync()
                .ConfigureAwait(false);

        }

        public async Task<RefreshWebToken?> GetActiveTokenAsync(string userUpnHash)
        {
            return await DbContext.RefreshWebTokens
                .FirstOrDefaultAsync(rt => rt.UserUpnHash == userUpnHash && !rt.IsUsed && !rt.IsRevoked);
        }

        public Task SaveChangesAsync()
        {
            return DbContext.SaveChangesAsync();
        }

        public Task ApplyPatchAsync(string tokenId, Action<EntityEntry<RefreshWebToken>> patch)
        {
            var token = new RefreshWebToken(tokenId);
            DbContext.Attach(token);
            var entry = DbContext.Entry(token);
            patch(entry);
            return Task.CompletedTask;
        }

        public async Task<List<RefreshWebToken>> GetAllValidTokensAsync()
        {
            return await DbContext.RefreshWebTokens
                .Where(rt =>
                    !rt.IsUsed &&
                    !rt.IsRevoked &&
                    rt.ExpiresAt > DateTimeOffset.UtcNow)
                .ToListAsync();
        }

        public async Task PatchCircuitIdIfMissingAsync(string tokenId, string circuitId)
        {
            var currentCircuitId = await DbContext.RefreshWebTokens
                .AsNoTracking()
                .Where(t => t.Id == tokenId)
                .Select(t => t.CircuitId)
                .FirstOrDefaultAsync();

            if (!string.IsNullOrWhiteSpace(currentCircuitId))
                return;

            var token = new RefreshWebToken(tokenId);
            DbContext.Attach(token);

            var entry = DbContext.Entry(token);
            entry.Property(x => x.CircuitId).CurrentValue = circuitId;
            entry.Property(x => x.CircuitId).IsModified = true;

            await DbContext.SaveChangesAsync();
        }

    }
}
