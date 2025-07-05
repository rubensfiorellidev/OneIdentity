using MassTransit;

#nullable disable
namespace OneID.Application.Messaging.Sagas.Contracts.Events
{
    public record KeycloakUserFailed : CorrelatedBy<Guid>
    {
        public Guid CorrelationId { get; init; }
        public string FaultReason { get; init; }
        public DateTimeOffset OccurredAt { get; init; } = DateTimeOffset.UtcNow;

    }
}
