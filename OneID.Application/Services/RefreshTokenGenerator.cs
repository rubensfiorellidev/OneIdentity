using OneID.Application.Interfaces.Tokens;
using System.Security.Cryptography;

namespace OneID.Application.Services
{
    public class RefreshTokenGenerator : IRefreshTokenGenerator
    {
        public string Generate()
        {
            var randomBytes = new byte[64];

            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomBytes);

            return ToBase64Url(randomBytes);

            //return Base64UrlEncoder.Encode(randomBytes);
        }

        private static string ToBase64Url(byte[] bytes)
        {
            return Convert.ToBase64String(bytes)
                .TrimEnd('=')
                .Replace('+', '-')
                .Replace('/', '_');
        }
    }

}
