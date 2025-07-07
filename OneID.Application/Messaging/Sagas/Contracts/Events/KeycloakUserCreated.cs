using MassTransit;
using OneID.Application.DTOs.Admission;

#nullable disable
namespace OneID.Application.Messaging.Sagas.Contracts.Events
{
    public record KeycloakUserCreated : CorrelatedBy<Guid>
    {
        public Guid CorrelationId { get; init; }
        public KeycloakPayload KeycloakPayload { get; init; }
    }
}
