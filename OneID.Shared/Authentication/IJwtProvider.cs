using Microsoft.IdentityModel.Tokens;
using OneID.Domain.Results;

#nullable disable
namespace OneID.Shared.Authentication
{
    public interface IJwtProvider
    {
        Task<AuthResult> GenerateTokenAsync(string userId, string preferredUsername = null, string email = null, string name = null);
        Task<string> EnsureKeysAsync();
        string GenerateAcceptanceToken(Dictionary<string, object> claims, TimeSpan? validFor);
        public RsaSecurityKey GetPublicKey();
        IDictionary<string, object> DecodeToken(string token);

    }
}
