using MassTransit;

namespace OneID.Application.Messaging.Sagas.Contracts.Events
{
    public record CommitLoginRequested : CorrelatedBy<Guid>
    {
        public Guid CorrelationId { get; init; }
    }
}
