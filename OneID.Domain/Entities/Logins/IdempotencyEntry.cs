namespace OneID.Domain.Entities.Logins
{
    public sealed class IdempotencyEntry
    {
        public IdempotencyEntry(string key, string payload)
        {
            Key = key;
            Payload = payload;
            CreatedAtUtc = DateTimeOffset.UtcNow;
        }

        public string Key { get; private set; }
        public string Payload { get; private set; }
        public DateTimeOffset CreatedAtUtc { get; private set; }

    }
}
