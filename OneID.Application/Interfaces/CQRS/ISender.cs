using OneID.Domain.Interfaces;

namespace OneID.Application.Interfaces.CQRS
{
    public interface ICommandHandler<in TCommand>
        where TCommand : ICommand<IOperationResult>
    {
        Task<IOperationResult> Handle(TCommand command, CancellationToken cancellationToken);
    }

    public interface ISender
    {
        Task<IOperationResult> SendAsync<TCommand>(TCommand command, CancellationToken cancellationToken)
            where TCommand : ICommand<IOperationResult>;
    }

}
