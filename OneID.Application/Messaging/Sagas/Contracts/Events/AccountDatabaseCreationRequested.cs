using MassTransit;
#nullable disable
namespace OneID.Application.Messaging.Sagas.Contracts.Events
{
    public record AccountDatabaseCreationRequested : CorrelatedBy<Guid>
    {
        public Guid CorrelationId { get; init; }
    }
}
