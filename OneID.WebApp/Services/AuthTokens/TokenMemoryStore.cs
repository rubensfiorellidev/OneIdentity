using OneID.WebApp.Interfaces;
using System.Collections.Concurrent;

namespace OneID.WebApp.Services.AuthTokens
{
    public sealed class TokenMemoryStore : ITokenMemoryStore
    {
        private readonly ConcurrentDictionary<string, CircuitTokenData> _tokens = new();


        public void SetToken(string circuitId, string refreshToken, DateTimeOffset expiresAt)
        {
            _tokens[circuitId] = new CircuitTokenData
            {
                RefreshToken = refreshToken,
                ExpiresAt = expiresAt
            };
        }

        public CircuitTokenData? GetToken(string circuitId)
        {
            _tokens.TryGetValue(circuitId, out var data);
            return data;
        }

        public void RemoveToken(string circuitId)
        {
            _tokens.TryRemove(circuitId, out _);
        }

    }
}
