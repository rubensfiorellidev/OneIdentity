using OneID.Application.DTOs.Admission;
using OneID.Application.Interfaces.CQRS;
using OneID.Domain.Interfaces;

#nullable disable
namespace OneID.Application.Commands
{
    public sealed record CreateAccountStagingCommand : ICommand<IOperationResult>
    {
        public AccountRequest Request { get; }
        public string CreatedBy { get; }

        public CreateAccountStagingCommand(AccountRequest request, string createdBy)
        {
            Request = request;
            CreatedBy = createdBy;
        }
    }

}
