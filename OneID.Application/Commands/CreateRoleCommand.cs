using OneID.Application.Interfaces.CQRS;
using OneID.Domain.Results;

namespace OneID.Application.Commands
{
    public record CreateRoleCommand(string Name, string Description, string CreatedBy) : ICommand<IResult>;

}
