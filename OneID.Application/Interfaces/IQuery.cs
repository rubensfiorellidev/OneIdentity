using MediatR;

namespace OneID.Application.Interfaces
{
    public interface IQuery<IResult> : IRequest<IResult> { }

}
