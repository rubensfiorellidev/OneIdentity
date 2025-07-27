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
            string token,
            string tokenSalt,
            DateTimeOffset createdAt,
            DateTimeOffset expiresAt,
            bool isRevoked,
            bool isUsed,
            string userAgent,
            string ipAddress)
        {
            Id = Ulid.NewUlid().ToString();
            UserUpnHash = userUpnHash;
            Jti = jti;
            Token = token;
            TokenSalt = tokenSalt;
            CreatedAt = createdAt;
            ExpiresAt = expiresAt;
            IsRevoked = isRevoked;
            IsUsed = isUsed;
            UserAgent = userAgent;
            IpAddress = ipAddress;
        }

        public string Id { get; init; }
        public string UserUpnHash { get; init; }
        public string Jti { get; init; }
        public string Token { get; init; }
        public string TokenSalt { get; init; }
        public DateTimeOffset CreatedAt { get; init; }
        public DateTimeOffset ExpiresAt { get; init; }
        public bool IsRevoked { get; init; }
        public bool IsUsed { get; init; }
        public string UserAgent { get; init; }
        public string IpAddress { get; init; }
    }
}
