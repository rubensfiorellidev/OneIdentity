using Microsoft.IdentityModel.Tokens;
using OneID.Domain.Results;

namespace OneID.Shared.Authentication
{
    public interface IJwtProvider
    {
        Task<AuthResult> GenerateTokenAsync(string upn);
        Task<string> EnsureKeysAsync();
        string GenerateAcceptanceToken(Dictionary<string, object> claims, TimeSpan validFor);
        public RsaSecurityKey GetPublicKey();
        IDictionary<string, object> DecodeToken(string token);

    }
}
