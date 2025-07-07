using MediatR;
using OneID.Application.Interfaces.Services;

namespace OneID.Application.Interfaces.CQRS
{
    public interface ICommand : IRequest<IResult> { }

}
