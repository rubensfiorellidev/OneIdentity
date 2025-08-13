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
        public DateTimeOffset UpdatedAt { get; set; }
        public string FaultReason { get; set; }
        public string LastEvent { get; set; }
        public byte[] Version { get; set; }

        public bool LoginAllocated { get; set; }
        public string Login { get; set; }
        public string CorporateEmail { get; set; }

        public bool KeycloakCreated { get; set; }
        public string KeycloakUserId { get; set; }

        public bool DatabaseCreated { get; set; }
        public string DatabaseId { get; set; }

        public bool AzureCreated { get; set; }
        public string AzureUserId { get; set; }

        public KeycloakPayload KeycloakData { get; set; } = default!;
        public UserAccountPayload AccountData { get; set; } = default!;


    }
}
