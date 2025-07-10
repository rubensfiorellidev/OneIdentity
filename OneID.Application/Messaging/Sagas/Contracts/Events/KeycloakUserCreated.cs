using MassTransit;
using OneID.Application.DTOs.Admission;

#nullable disable
namespace OneID.Application.Messaging.Sagas.Contracts.Events
{
    public record KeycloakUserCreated : CorrelatedBy<Guid>
    {
        public Guid CorrelationId { get; init; }
        public KeycloakPayload KeycloakPayload { get; init; }

        public string Cpf { get; init; }
        public string FullName { get; init; }
        public string PositionHeldId { get; init; }
    }
}

