using Microsoft.EntityFrameworkCore.ChangeTracking;
using OneID.Domain.Entities.JwtWebTokens;

namespace OneID.Application.Interfaces.Repositories
{
    public interface IRefreshTokenRepository
    {
        Task<RefreshWebToken?> GetActiveTokenAsync(string userUpn);
        Task AddAsync(RefreshWebToken token);
        Task SaveChangesAsync();
        Task ApplyPatchAsync(string tokenId, Action<EntityEntry<RefreshWebToken>> patch);
        Task<List<RefreshWebToken>> GetAllValidTokensAsync();
    }

}
