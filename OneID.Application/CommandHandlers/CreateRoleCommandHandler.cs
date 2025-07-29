using OneID.Application.Commands;
using OneID.Application.Interfaces.CQRS;
using OneID.Application.Interfaces.Repositories;
using OneID.Domain.Entities.UserContext;
using OneID.Domain.Interfaces;
using OneID.Domain.Results;

namespace OneID.Application.CommandHandlers
{
    public class CreateRoleCommandHandler : ICommandHandler<CreateRoleCommand, IOperationResult>
    {
        private readonly IRoleWriterRepository _repository;

        public CreateRoleCommandHandler(IRoleWriterRepository repository)
        {
            _repository = repository;
        }

        public async Task<IOperationResult> Handle(CreateRoleCommand request, CancellationToken cancellationToken)
        {
            var role = new Role(request.Name, request.Description);

            role.SetCreatedBy(request.CreatedBy);

            if (!role.IsValid())
            {
                var errorMessage = string.Join(" | ", role.Notifications.Select(n => n.Message));
                return Result.Failure(errorMessage);
            }

            await _repository.AddAsync(role, cancellationToken);

            return Result.Success();
        }
    }

}
