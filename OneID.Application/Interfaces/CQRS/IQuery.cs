using MediatR;

namespace OneID.Application.Interfaces.CQRS
{
    public interface IQuery<IOperationResult> : IRequest<IOperationResult>
    {

    }
}
