using Microsoft.Extensions.DependencyInjection;
using OneID.Application.Interfaces.CQRS;
using OneID.Domain.Results;

namespace OneID.Application.Services
{
    public class Sender : ISender
    {
        private readonly IServiceProvider _serviceProvider;

        public Sender(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public Task<IResult> SendAsync<TCommand>(TCommand command, CancellationToken cancellationToken)
            where TCommand : ICommand<IResult>
        {
            var handler = _serviceProvider.GetRequiredService<ICommandHandler<TCommand, IResult>>();
            return handler.Handle(command, cancellationToken);
        }
    }

}
