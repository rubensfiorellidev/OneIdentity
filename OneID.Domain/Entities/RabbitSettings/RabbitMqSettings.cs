#nullable disable
namespace OneID.Domain.Entities.RabbitSettings
{
    public sealed class RabbitMqSettings
    {
        public string HostName { get; set; }
        public int Port { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }

        public string SapExchangeName { get; set; }
        public string SyncExchangeName { get; set; }
        public string DeadLetterExchangeName { get; set; }
        public string DeadLetterQueueName { get; set; }
        public string ExchangeSyncQueue { get; set; }
        public string ExchangeSyncDelayedQueue { get; set; }

        public string ExchangeType { get; set; }
        public bool IndentedFormatting { get; set; }

        private int _heartbeat = 60;
        public int Heartbeat
        {
            get => _heartbeat;
            set => _heartbeat = value > 0 ? value : 60;
        }

        public int ReconnectionInterval { get; set; }

        public string QueueNameRequest { get; set; }
        public string QueueNameResponse { get; set; }

        public string[] RequestRoutingKeys { get; set; }
        public string[] ResponseRoutingKeys { get; set; }
        public string[] SyncRequestRoutingKeys { get; set; }
        public string[] SyncResponseRoutingKeys { get; set; }
        public string VirtualHost { get; set; }

        public SafeBindings SafeBindings { get; set; }
    }

    public class SafeBindings
    {
        public string Queue { get; set; }
        public string Exchange { get; set; }
        public string ForbiddenRoutingKey { get; set; }
    }

}
