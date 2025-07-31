using OneID.Application.Interfaces.Repositories;
using OneID.Application.Interfaces.Services;
using OneID.Application.Interfaces.Tokens;
using OneID.Domain.Entities.JwtWebTokens;
using OneID.Domain.Interfaces;
using System.Security.Cryptography;

#nullable disable
namespace OneID.Application.Services.RefreshTokens
{
    public sealed class RefreshTokenService : IRefreshTokenService
    {
        private readonly IRefreshTokenRepository _repository;
        private readonly IHashService _hash;
        private readonly IRefreshTokenGenerator _tokenGenerator;

        public RefreshTokenService(IRefreshTokenRepository repository,
                                   IHashService hash,
                                   IRefreshTokenGenerator tokenGenerator)
        {
            _repository = repository;
            _hash = hash;
            _tokenGenerator = tokenGenerator;
        }

        public async Task<RefreshWebToken> GenerateRefreshTokenAsync(string userUpnHash,
                                                                     string jti,
                                                                     string ip = null,
                                                                     string userAgent = null,
                                                                     string circuitId = null)
        {

            var rawToken = _tokenGenerator.Generate();

            var saltBytes = RandomNumberGenerator.GetBytes(16);
            var saltBase64 = Convert.ToBase64String(saltBytes);

            var tokenHash = await _hash.ComputeSha3HashAsync(rawToken + saltBase64);


            var current = await _repository.GetActiveTokenAsync(userUpnHash);
            if (current != null)
                _ = current with { IsUsed = true };

            var refreshToken = new RefreshWebToken(
                userUpnHash,
                jti,
                tokenHash,
                saltBase64,
                DateTimeOffset.UtcNow,
                DateTimeOffset.UtcNow.AddDays(7),
                false,
                false,
                userAgent,
                ipAddress: ip,
                circuitId: circuitId
            );

            await _repository.AddAsync(refreshToken);
            await _repository.SaveChangesAsync();

            return refreshToken with { TokenHash = rawToken };
        }

        public async Task<RefreshWebToken> GetRefreshTokenAsync(string rawToken)
        {
            var candidates = await _repository.GetAllValidTokensAsync();

            foreach (var token in candidates)
            {
                var hash = await _hash.ComputeSha3HashAsync(rawToken + token.TokenSalt);
                if (hash == token.TokenHash)
                    return token;
            }

            return null;
        }


        public async Task MarkRefreshTokenAsUsedAsync(string refreshToken)
        {
            var token = await GetRefreshTokenAsync(refreshToken);
            if (token != null)
            {
                await _repository.ApplyPatchAsync(token.Id, entry =>
                {
                    entry.Property(x => x.IsUsed).CurrentValue = true;
                    entry.Property(x => x.IsUsed).IsModified = true;
                });

                await _repository.SaveChangesAsync();
            }
        }

        public async Task PatchCircuitIdIfMissingAsync(string tokenId, string circuitId)
        {
            await _repository.PatchCircuitIdIfMissingAsync(tokenId, circuitId);
        }

        public async Task RevokeRefreshTokenAsync(string refreshToken)
        {
            var token = await GetRefreshTokenAsync(refreshToken);
            if (token != null)
            {
                await _repository.ApplyPatchAsync(token.Id, entry =>
                {
                    entry.Property(x => x.IsRevoked).CurrentValue = true;
                    entry.Property(x => x.IsRevoked).IsModified = true;
                });

                await _repository.SaveChangesAsync();
            }
        }
    }

}
