using MassTransit;

#nullable disable
namespace OneID.Application.Messaging.Sagas.Contracts.Events
{
    public record KeycloakUserCreated : CorrelatedBy<Guid>
    {
        public Guid CorrelationId { get; init; }
        public string Username { get; init; }
    }
}
