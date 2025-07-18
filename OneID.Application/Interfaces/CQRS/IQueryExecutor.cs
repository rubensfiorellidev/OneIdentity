namespace OneID.Application.Interfaces.CQRS
{
    public interface IQueryExecutor
    {
        Task<TResponse> SendQueryAsync<TQuery, TResponse>(TQuery query, CancellationToken cancellationToken)
            where TQuery : IQuery<TResponse>;
    }

}
