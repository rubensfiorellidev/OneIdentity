using OneID.Domain.Results;

namespace OneID.Application.Interfaces.CQRS
{
    public interface ICommand : ICommand<IResult> { }

    public interface ICommand<TResponse> { }


}
