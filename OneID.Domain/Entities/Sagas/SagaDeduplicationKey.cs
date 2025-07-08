namespace OneID.Domain.Entities.Sagas
{
    public sealed class SagaDeduplicationKey
    {

        public Guid Id { get; set; } = Guid.NewGuid();
        public string BusinessKey { get; set; } = null!;
        public string ProcessName { get; set; } = null!;
        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

    }
}
