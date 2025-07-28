namespace OneID.WebApp.Services
{
    using Microsoft.AspNetCore.Components.Server.Circuits;
    using System.Threading.Channels;

    public sealed class TokenCircuitHandler : CircuitHandler
    {
        private static readonly Channel<Circuit> _circuitChannel = Channel.CreateUnbounded<Circuit>();
        private static readonly HashSet<Circuit> _activeCircuits = [];

        public override Task OnCircuitOpenedAsync(Circuit circuit, CancellationToken cancellationToken)
        {
            _activeCircuits.Add(circuit);
            return Task.CompletedTask;
        }

        public override Task OnCircuitClosedAsync(Circuit circuit, CancellationToken cancellationToken)
        {
            _activeCircuits.Remove(circuit);
            return Task.CompletedTask;
        }

        public static bool IsCircuitActive(Circuit circuit)
        {
            return _activeCircuits.Contains(circuit);
        }
    }

}
