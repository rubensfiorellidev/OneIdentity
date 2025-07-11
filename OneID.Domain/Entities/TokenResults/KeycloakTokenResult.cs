using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;

#nullable disable
namespace OneID.Domain.Entities.TokenResults
{
    public class KeycloakTokenResult
    {
        [JsonProperty("access_token")]
        public string AccessToken { get; set; }

        [JsonProperty("expires_in")]
        public int ExpiresIn { get; set; }

        [JsonProperty("refresh_token")]
        public string RefreshToken { get; set; }

        [JsonProperty("refresh_expires_in")]
        public int RefreshExpiresIn { get; set; }

        [JsonIgnore]
        public string Sub => GetClaim("sub");

        [JsonIgnore]
        public string PreferredUsername => GetClaim("preferred_username");

        private string GetClaim(string claimType)
        {
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(AccessToken);
            return jwtToken?.Claims?.FirstOrDefault(c => c.Type == claimType)?.Value ?? string.Empty;
        }
    }

}
