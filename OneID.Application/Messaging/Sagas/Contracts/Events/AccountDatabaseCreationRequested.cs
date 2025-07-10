using MassTransit;
using OneID.Application.DTOs.Admission;
#nullable disable
namespace OneID.Application.Messaging.Sagas.Contracts.Events
{
    public record AccountDatabaseCreationRequested : CorrelatedBy<Guid>
    {
        public Guid CorrelationId { get; init; }
        public UserAccountPayload DatabasePayload { get; set; }

    }
}
