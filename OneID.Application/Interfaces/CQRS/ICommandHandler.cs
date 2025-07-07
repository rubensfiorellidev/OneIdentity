using MediatR;
using OneID.Application.Interfaces.Services;

namespace OneID.Application.Interfaces.CQRS
{
    public interface ICommandHandler<in TCommand, out W>
      : IRequestHandler<TCommand, IResult> where TCommand
      : ICommand where W : IResult
    {

    }

}
