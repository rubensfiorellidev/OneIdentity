using MediatR;
using OneID.Domain.Interfaces;

namespace OneID.Application.Interfaces.CQRS
{
    public interface ICommandHandler<in TCommand, out W>
        : IRequestHandler<TCommand, IOperationResult> where TCommand
        : ICommand where W : IOperationResult
    {

    }

}
