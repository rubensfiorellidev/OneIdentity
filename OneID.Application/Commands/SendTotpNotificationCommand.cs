using OneID.Application.Interfaces.CQRS;
using OneID.Domain.Results;

namespace OneID.Application.Commands
{
    public record SendTotpNotificationCommand : ICommand<IResult>
    {
        public Guid CorrelationId { get; init; }
        public string EmailOperador { get; init; }
        public string NomeOperador { get; init; }
        public string TelefoneOperador { get; init; }

        public SendTotpNotificationCommand(Guid correlationId, string email, string nome, string telefone)
        {
            CorrelationId = correlationId;
            EmailOperador = email;
            NomeOperador = nome;
            TelefoneOperador = telefone;
        }
    }

}
