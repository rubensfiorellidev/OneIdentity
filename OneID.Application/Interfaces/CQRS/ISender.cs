using OneID.Domain.Results;

namespace OneID.Application.Interfaces.CQRS
{
    public interface ICommandHandler<in TCommand>
        where TCommand : ICommand<IResult>
    {
        Task<IResult> Handle(TCommand command, CancellationToken cancellationToken);
    }

    public interface ISender
    {
        Task<IResult> SendAsync<TCommand>(TCommand command, CancellationToken cancellationToken)
            where TCommand : ICommand<IResult>;
    }

}
