using MediatR;

namespace OneID.Application.Interfaces
{
    public interface ICommandHandler<in TCommand, out W>
      : IRequestHandler<TCommand, IResult> where TCommand
      : ICommand where W : IResult
    {

    }

}
