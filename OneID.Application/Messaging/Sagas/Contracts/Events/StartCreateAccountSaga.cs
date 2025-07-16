using MassTransit;
using OneID.Application.DTOs.Admission;


#nullable disable
namespace OneID.Application.Messaging.Sagas.Contracts.Events
{
    public record StartCreateAccountSaga : CorrelatedBy<Guid>
    {
        public Guid CorrelationId { get; set; }
        public KeycloakPayload KeycloakPayload { get; set; }

    }

}
