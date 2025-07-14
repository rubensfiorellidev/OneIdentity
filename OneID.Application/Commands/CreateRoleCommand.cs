using OneID.Application.Interfaces.CQRS;
using OneID.Application.Interfaces.Services;

namespace OneID.Application.Commands
{
    public record CreateRoleCommand(string Name, string Description, string CreatedBy) : ICommand<IResult>;

}
