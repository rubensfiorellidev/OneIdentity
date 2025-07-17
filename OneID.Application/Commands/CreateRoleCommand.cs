using OneID.Application.Interfaces.CQRS;
using OneID.Domain.Interfaces;

namespace OneID.Application.Commands
{
    public record CreateRoleCommand(string Name, string Description, string CreatedBy) : ICommand<IResult>;

}
