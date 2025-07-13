using Microsoft.Extensions.DependencyInjection;
using OneID.Application.Interfaces.CQRS;
using OneID.Application.Interfaces.Services;

namespace OneID.Application.Services
{
    public class CommandDispatcher : ICommandDispatcher
    {
        private readonly IServiceProvider _serviceProvider;

        public CommandDispatcher(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public Task<IResult> DispatchAsync<TCommand>(TCommand command, CancellationToken cancellationToken)
            where TCommand : ICommand<IResult>
        {
            var handler = _serviceProvider.GetRequiredService<ICommandHandler<TCommand, IResult>>();
            return handler.Handle(command, cancellationToken);
        }
    }

}
