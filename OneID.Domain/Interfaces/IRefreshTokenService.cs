using OneID.Domain.Entities.JwtWebTokens;

namespace OneID.Domain.Interfaces
{
    public interface IRefreshTokenService
    {
        Task<RefreshWebToken> GenerateRefreshTokenAsync(string userUpn, string jti, string? ip = null, string? userAgent = null, string? circuitId = null);
        Task<RefreshWebToken?> GetRefreshTokenAsync(string refreshToken);
        Task RevokeRefreshTokenAsync(string token);
        Task MarkRefreshTokenAsUsedAsync(string token);

    }
}
