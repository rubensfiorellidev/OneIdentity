using OneID.Application.Interfaces.Services;

namespace OneID.Application.Interfaces.CQRS
{
    public interface ICommand : ICommand<IResult> { }

    public interface ICommand<TResponse> { }


}
