using OneID.WebApp.Services.AuthTokens;

namespace OneID.WebApp.Interfaces
{
    public interface ITokenMemoryStore
    {
        void SetToken(string circuitId, string refreshToken, DateTimeOffset expiresAt);
        CircuitTokenData? GetToken(string circuitId);
        void RemoveToken(string circuitId);
    }

}
