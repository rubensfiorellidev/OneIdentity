using OneID.Application.Interfaces.CQRS;

#nullable disable
namespace OneID.Application.Commands
{
    public record RegisterSessionCommand : ICommand
    {
        public string CircuitId { get; init; }
        public string IpAddress { get; init; }
        public string UpnOrName { get; init; }
        public string UserAgent { get; init; }
        public DateTimeOffset LastActivity { get; init; }
        public DateTimeOffset ExpiresAt { get; init; }
    }

}
