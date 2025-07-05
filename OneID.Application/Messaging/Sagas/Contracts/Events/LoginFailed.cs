#nullable disable
using MassTransit;

namespace OneID.Application.Messaging.Sagas.Contracts.Events
{
    public record LoginFailed : CorrelatedBy<Guid>
    {
        public Guid CorrelationId { get; init; }
        public string FaultReason { get; init; }
        public DateTimeOffset OccurredAt { get; init; } = DateTimeOffset.UtcNow;
    }
}
