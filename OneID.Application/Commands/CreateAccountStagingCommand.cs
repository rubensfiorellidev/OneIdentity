using OneID.Application.DTOs.Admission;
using OneID.Application.Interfaces.CQRS;
#nullable disable
namespace OneID.Application.Commands
{
    public sealed record CreateAccountStagingCommand : ICommand
    {
        public AccountRequest Request { get; }
        public CreateAccountStagingCommand(AccountRequest request)
        {
            Request = request;
        }

    }

}
