using OneID.Application.Interfaces.Services;

namespace OneID.Application.Interfaces.CQRS
{
    public interface ICommandHandler<in TCommand>
        where TCommand : ICommand<IResult>
    {
        Task<IResult> Handle(TCommand command, CancellationToken cancellationToken);
    }

    public interface ICommandDispatcher
    {
        Task<IResult> DispatchAsync<TCommand>(TCommand command, CancellationToken cancellationToken)
            where TCommand : ICommand<IResult>;
    }

}
