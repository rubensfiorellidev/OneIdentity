using MassTransit;
using OneID.Application.DTOs.Admission;

#nullable disable
namespace OneID.Application.Messaging.Sagas.Contracts.Events
{
    public record CreateUserAccountRequested : CorrelatedBy<Guid>
    {
        public Guid CorrelationId { get; init; }
        public UserAccountPayload DatabasePayload { get; init; }
    }

}
