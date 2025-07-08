using MediatR;

namespace OneID.Application.Interfaces
{
    public interface IQueryHandler<TQuery, TResponse>
        : IRequestHandler<TQuery, TResponse> where TQuery
        : IQuery<TResponse>
    {

    }
}
