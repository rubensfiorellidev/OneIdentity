using MediatR;

namespace OneID.Application.Interfaces.CQRS
{
    public interface IQueryHandler<TQuery, TResponse>
        : IRequestHandler<TQuery, TResponse> where TQuery
        : IQuery<TResponse>
    {

    }
}
