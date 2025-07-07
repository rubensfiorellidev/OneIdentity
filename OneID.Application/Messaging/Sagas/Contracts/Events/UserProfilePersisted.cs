using MassTransit;

#nullable disable
namespace OneID.Application.Messaging.Sagas.Contracts.Events
{
    public record UserProfilePersisted : CorrelatedBy<Guid>
    {
        public Guid CorrelationId { get; init; }
        public string DatabaseId { get; init; }
    }

}
