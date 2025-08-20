#nullable disable
namespace OneID.Domain.Entities.TotpOptions
{
    public sealed class TotpCodeUseEntity
    {
        public TotpCodeUseEntity(string userId,
                                 long step,
                                 DateTimeOffset expiresAt)
        {
            Id = Ulid.NewUlid().ToString();
            UserId = userId;
            Step = step;
            CreatedAt = DateTimeOffset.UtcNow;
            ExpiresAt = expiresAt;
        }

        public string Id { get; private set; }
        public string UserId { get; private set; }
        public long Step { get; private set; }
        public DateTimeOffset CreatedAt { get; private set; }
        public DateTimeOffset ExpiresAt { get; private set; }

    }
}
