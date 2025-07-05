using RabbitMQ.Client;

namespace OneID.Domain.Interfaces
{
    public interface IRabbitMqConnection
    {
        Task<IConnection> GetConnectionAsync();
        bool IsConnected();

    }
}
