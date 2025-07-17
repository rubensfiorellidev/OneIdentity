using MassTransit;

#nullable disable
namespace OneID.Application.Messaging.Sagas.Contracts.Events
{
    public record AzureUserCreationRequested : CorrelatedBy<Guid>
    {
        public Guid CorrelationId { get; init; }
        public string FirstName { get; init; }
        public string LastName { get; init; }
        public string Email { get; init; }
        public string Login { get; init; }
        public string Password { get; init; }
        public string JobTitle { get; init; }
        public string Department { get; init; }
        public string Registry { get; init; }
        public string ManagerLogin { get; init; }

    }
}
