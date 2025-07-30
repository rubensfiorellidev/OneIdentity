#nullable disable
namespace OneID.Domain.Entities.JwtWebTokens
{

    public record RefreshWebToken
    {
        public RefreshWebToken(string id)
        {
            Id = id;
        }

        public RefreshWebToken(
            string userUpnHash,
            string jti,
            string tokenHash,
            string tokenSalt,
            DateTimeOffset createdAt,
            DateTimeOffset expiresAt,
            bool isRevoked,
            bool isUsed,
            string userAgent,
            string ipAddress,
            string circuitId)
        {
            Id = Ulid.NewUlid().ToString();
            UserUpnHash = userUpnHash;
            Jti = jti;
            TokenHash = tokenHash;
            TokenSalt = tokenSalt;
            CreatedAt = createdAt;
            ExpiresAt = expiresAt;
            IsRevoked = isRevoked;
            IsUsed = isUsed;
            UserAgent = userAgent;
            IpAddress = ipAddress;
            CircuitId = circuitId;
        }

        public string Id { get; init; }
        public string UserUpnHash { get; init; }
        public string Jti { get; init; }
        public string TokenHash { get; init; }
        public string TokenSalt { get; init; }
        public DateTimeOffset CreatedAt { get; init; }
        public DateTimeOffset ExpiresAt { get; init; }
        public bool IsRevoked { get; init; }
        public bool IsUsed { get; init; }
        public string UserAgent { get; init; }
        public string IpAddress { get; init; }
        public string CircuitId { get; init; }
    }
}
