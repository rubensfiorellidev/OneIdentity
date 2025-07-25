﻿using OneID.Application.Commands;
using OneID.Application.Interfaces.CQRS;
using OneID.Application.Interfaces.Repositories;
using OneID.Application.Interfaces.Services;
using OneID.Application.Results;
using OneID.Domain.Entities.UserContext;

namespace OneID.Application.CommandHandlers
{
    public class CreateRoleCommandHandler : ICommandHandler<CreateRoleCommand, IResult>
    {
        private readonly IRoleWriterRepository _repository;

        public CreateRoleCommandHandler(IRoleWriterRepository repository)
        {
            _repository = repository;
        }

        public async Task<IResult> Handle(CreateRoleCommand request, CancellationToken cancellationToken)
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
