using OneID.Domain.Interfaces;

namespace OneID.Application.Interfaces.CQRS
{
    public interface ICommand : ICommand<IOperationResult> { }

    public interface ICommand<TResponse> { }


}
