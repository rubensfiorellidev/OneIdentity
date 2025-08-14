using MassTransit;

namespace OneID.Application.Messaging.Sagas.Contracts.Events
{
    public record ReleaseLoginRequested : CorrelatedBy<Guid>
    {
        public Guid CorrelationId { get; init; }
    }
}
