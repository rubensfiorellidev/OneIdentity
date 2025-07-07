using MassTransit;
using OneID.Application.DTOs.Admission;

#nullable disable
namespace OneID.Application.Messaging.Sagas.Contracts
{
    public sealed class AccountSagaState : SagaStateMachineInstance
    {
        public Guid CorrelationId { get; set; }
        public string CurrentState { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public string FaultReason { get; set; }
        public byte[] Version { get; set; }

        public KeycloakPayload Payload { get; set; }
        public UserAccountPayload DatabasePayload { get; set; }

    }
}
