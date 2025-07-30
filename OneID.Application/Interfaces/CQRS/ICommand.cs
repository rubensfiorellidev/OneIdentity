using MediatR;
using OneID.Domain.Interfaces;

namespace OneID.Application.Interfaces.CQRS
{
    public interface ICommand : IRequest<IOperationResult> { }



}
