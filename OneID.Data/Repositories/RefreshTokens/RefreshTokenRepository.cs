using Microsoft.EntityFrameworkCore;
using OneID.Application.Interfaces;
using OneID.Data.Interfaces;
using OneID.Domain.Entities.JwtWebTokens;

namespace OneID.Data.Repositories.RefreshTokens
{
    public sealed class RefreshTokenRepository : IRefreshTokenRepository
    {
        private readonly IOneDbContextFactory _dbContextFactory;
        public RefreshTokenRepository(IOneDbContextFactory dbContextFactory)
        {
            _dbContextFactory = dbContextFactory;
        }

        public async Task AddAsync(RefreshWebToken token)
        {
            await using var dbContext = _dbContextFactory.CreateDbContext();

            await dbContext.RefreshWebTokens.AddAsync(token);

        }

        public async Task<RefreshWebToken?> GetActiveTokenAsync(string userUpn)
        {
            await using var dbContext = _dbContextFactory.CreateDbContext();

            return await dbContext.RefreshWebTokens
                .FirstOrDefaultAsync(rt => rt.UserUpn == userUpn && !rt.IsUsed && !rt.IsRevoked);
        }

        public async Task<RefreshWebToken?> GetByTokenAsync(string token)
        {
            await using var dbContext = _dbContextFactory.CreateDbContext();

            return await dbContext.RefreshWebTokens
                .FirstOrDefaultAsync(rt => rt.Token == token && !rt.IsUsed && !rt.IsRevoked);

        }

        public async Task SaveChangesAsync()
        {
            await using var dbContext = _dbContextFactory.CreateDbContext();

            await dbContext.SaveChangesAsync();

        }
    }
}
