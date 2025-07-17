using MassTransit;

#nullable disable
namespace OneID.Application.Messaging.Sagas.Contracts.Events
{
    public record AzureUserCreated : CorrelatedBy<Guid>
    {
        public Guid CorrelationId { get; init; }
        public string AzureUserId { get; init; }
    }
}
