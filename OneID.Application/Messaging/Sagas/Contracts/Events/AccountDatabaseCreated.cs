using MassTransit;

#nullable disable
namespace OneID.Application.Messaging.Sagas.Contracts.Events
{
    public record AccountDatabaseCreated : CorrelatedBy<Guid>
    {
        public Guid CorrelationId { get; init; }
        public string DatabaseId { get; init; }
        public string Login { get; init; }
        public DateTimeOffset CreatedAt { get; init; }
    }

}
