using MediatR;

namespace OneID.Application.Interfaces
{
    public interface ICommand : IRequest<IResult> { }

}
