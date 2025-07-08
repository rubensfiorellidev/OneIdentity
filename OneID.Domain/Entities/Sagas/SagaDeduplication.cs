namespace OneID.Domain.Entities.Sagas
{
    public class SagaDeduplication
    {
        public Guid CorrelationId { get; set; }

        public string ProcessName { get; set; } = null!;

        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    }
}
