using MediatR;

namespace OneID.Application.Interfaces.CQRS
{
    public interface IQuery<IResult> : IRequest<IResult> { }

}
