using MassTransit;

namespace OneID.Application.Messaging.Sagas.Contracts.Events
{
#nullable disable

    public record CreateLoginRequested : CorrelatedBy<Guid>
    {
        public Guid CorrelationId { get; init; }
        public string FirstName { get; init; }
        public string LastName { get; init; }

    }
}
