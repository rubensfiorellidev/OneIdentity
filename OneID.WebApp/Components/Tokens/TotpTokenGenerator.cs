using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

#nullable disable
namespace OneID.WebApp.Components.Tokens
{
    public sealed class TotpTokenGenerator : ITotpTokenGenerator
    {
        private readonly IConfiguration _config;

        public TotpTokenGenerator(IConfiguration config)
        {
            _config = config;
        }

        public string GenerateToken(Dictionary<string, object> claims, TimeSpan expiresIn)
        {
            var key = _config["Jwt:SecretKey"];
            if (string.IsNullOrWhiteSpace(key))
                throw new InvalidOperationException("Secret key ausente em appsettings");

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var jwtClaims = claims.Select(kvp => new Claim(kvp.Key, kvp.Value.ToString()));
            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: jwtClaims,
                expires: DateTime.UtcNow.Add(expiresIn),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }

}
