#nullable disable
namespace OneID.Domain.Entities.Logins
{
    public sealed class MfaChallengeEntity
    {
        public string Jti { get; private set; }
        public string UserId { get; private set; }
        public string CodeChallenge { get; set; }
        public string IpHash { get; set; }
        public string UserAgentHash { get; set; }
        public DateTimeOffset ExpiresAt { get; set; }
        public bool Used { get; set; }
        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
        public DateTimeOffset? ConsumedAt { get; set; }
        public byte[] RowVersion { get; set; }
    }
}
