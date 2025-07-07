using OneID.Domain.Entities.JwtWebTokens;

namespace OneID.Domain.Interfaces
{
    public interface IRefreshTokenService
    {
        Task<RefreshWebToken> GenerateRefreshTokenAsync(string userUpn, string jti);
        Task<RefreshWebToken?> GetRefreshTokenAsync(string token);
        Task RevokeRefreshTokenAsync(string token);
        Task MarkRefreshTokenAsUsedAsync(string token);

    }
}
