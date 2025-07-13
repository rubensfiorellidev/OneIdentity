using Microsoft.Extensions.DependencyInjection;
using OneID.Application.Interfaces.CQRS;

namespace OneID.Application.Services
{
    public class QueryDispatcher : IQueryDispatcher
    {
        private readonly IServiceProvider _serviceProvider;

        public QueryDispatcher(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public Task<TResponse> DispatchAsync<TQuery, TResponse>(TQuery query, CancellationToken cancellationToken)
            where TQuery : IQuery<TResponse>
        {
            var handler = _serviceProvider.GetRequiredService<IQueryHandler<TQuery, TResponse>>();
            return handler.Handle(query, cancellationToken);
        }
    }

}
