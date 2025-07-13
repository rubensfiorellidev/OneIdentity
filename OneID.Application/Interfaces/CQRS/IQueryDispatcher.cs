namespace OneID.Application.Interfaces.CQRS
{
    public interface IQueryDispatcher
    {
        Task<TResponse> DispatchAsync<TQuery, TResponse>(TQuery query, CancellationToken cancellationToken)
            where TQuery : IQuery<TResponse>;
    }

}
