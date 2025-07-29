using Microsoft.Extensions.DependencyInjection;
using OneID.Application.Interfaces.CQRS;
using OneID.Domain.Interfaces;

namespace OneID.Application.Services
{
    public class Sender : ISender
    {
        private readonly IServiceProvider _serviceProvider;

        public Sender(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public Task<IOperationResult> SendAsync<TCommand>(TCommand command, CancellationToken cancellationToken)
            where TCommand : ICommand<IOperationResult>
        {
            var handler = _serviceProvider.GetRequiredService<ICommandHandler<TCommand, IOperationResult>>();
            return handler.Handle(command, cancellationToken);
        }
    }

}
