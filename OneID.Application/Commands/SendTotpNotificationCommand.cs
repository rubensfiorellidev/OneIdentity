using OneID.Application.Interfaces.CQRS;
using OneID.Domain.Interfaces;

namespace OneID.Application.Commands
{
    public record SendTotpNotificationCommand : ICommand<IResult>
    {
        public Guid CorrelationId { get; init; }
        public string OperatorEmail { get; init; }
        public string OperatorName { get; init; }
        public string OperatorPhone { get; init; }

        public SendTotpNotificationCommand(Guid correlationId, string email, string name, string phone)
        {
            CorrelationId = correlationId;
            OperatorEmail = email;
            OperatorName = name;
            OperatorPhone = phone;
        }
    }

}
