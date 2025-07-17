using OneID.Application.DTOs.Admission;
using OneID.Application.Interfaces.CQRS;
using OneID.Domain.Results;
#nullable disable
namespace OneID.Application.Commands
{
    public sealed record CreateAccountStagingCommand : ICommand<IResult>
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
