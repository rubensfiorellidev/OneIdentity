using OneID.Application.Interfaces;
using OneID.Domain.Entities.JwtWebTokens;
using OneID.Shared.Authentication;
using System.Security.Cryptography;

namespace OneID.Application.Services.RefreshTokens
{
    public sealed class RefreshTokenService : IRefreshTokenService
    {
        private readonly IRefreshTokenRepository _repository;

        public RefreshTokenService(IRefreshTokenRepository repository)
        {
            _repository = repository;
        }

        private string GenerateRefreshTokenString()
        {
            var randomNumber = new byte[64];
            using (var numberGenerator = RandomNumberGenerator.Create())
            {
                numberGenerator.GetBytes(randomNumber);
            }
            return Convert.ToBase64String(randomNumber);
        }

        public async Task<RefreshWebToken> GenerateRefreshTokenAsync(string userUpn, string jti)
        {
            var current = await _repository.GetActiveTokenAsync(userUpn);
            if (current != null)
            {
                current.IsUsed = true;
            }

            var refreshToken = new RefreshWebToken
            {
                UserUpn = userUpn,
                Jti = jti,
                Token = GenerateRefreshTokenString(),
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddDays(7),
                IsUsed = false,
                IsRevoked = false
            };

            await _repository.AddAsync(refreshToken);
            await _repository.SaveChangesAsync();

            return refreshToken;
        }

        public async Task<RefreshWebToken?> GetRefreshTokenAsync(string token)
        {
            return await _repository.GetByTokenAsync(token);
        }

        public async Task MarkRefreshTokenAsUsedAsync(string token)
        {
            var refreshToken = await _repository.GetByTokenAsync(token);
            if (refreshToken != null)
            {
                refreshToken.IsUsed = true;
                await _repository.SaveChangesAsync();
            }
        }

        public async Task RevokeRefreshTokenAsync(string token)
        {
            var refreshToken = await _repository.GetByTokenAsync(token);
            if (refreshToken != null)
            {
                refreshToken.IsRevoked = true;
                await _repository.SaveChangesAsync();
            }
        }
    }

}
