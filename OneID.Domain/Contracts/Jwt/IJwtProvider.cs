using Microsoft.IdentityModel.Tokens;
using OneID.Domain.Results;


#nullable disable
namespace OneID.Domain.Contracts.Jwt
{
    public interface IJwtProvider
    {
        Task<AuthResult> GenerateAuthenticatedAccessTokenAsync(Guid keycloakUserId, string preferredUsername = null, string email = null, string name = null);
        Task<string> EnsureKeysAsync();
        string CreateBootstrapToken(Dictionary<string, object> claims, TimeSpan? validFor);
        public RsaSecurityKey GetPublicKey();
        IDictionary<string, object> DecodeToken(string token);
        bool ValidateRequestToken(string token);
        Task<(string Token, string RefreshToken, bool Success)> RefreshTokenAsync(string userUpn, string refreshToken);
        string GenerateInitialRequestTokenAsync(string username, Guid correlationId);
        string GenerateUserAccessTokenAsync(string username, Guid correlationId);
        string GenerateBootstrapTokenAsync();

    }
}
