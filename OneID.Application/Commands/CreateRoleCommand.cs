using OneID.Application.Interfaces.CQRS;

namespace OneID.Application.Commands
{
    public record CreateRoleCommand(string Name, string Description, string CreatedBy) : ICommand;

}
