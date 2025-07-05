using MassTransit;
using OneID.Application.DTOs.Admission;

#nullable disable
namespace OneID.Application.Messaging.Sagas.Contracts.Events
{
    public record KeycloakUserCreationRequested : CorrelatedBy<Guid>
    {
        public Guid CorrelationId { get; init; }
        public AdmissionPayload Payload { get; init; }
    }
}
