using OneID.Domain.Entities.JwtWebTokens;

namespace OneID.Application.Interfaces
{
    public interface IRefreshTokenRepository
    {
        Task<RefreshWebToken?> GetActiveTokenAsync(string userUpn);
        Task<RefreshWebToken?> GetByTokenAsync(string token);
        Task AddAsync(RefreshWebToken token);
        Task SaveChangesAsync();
    }

}
