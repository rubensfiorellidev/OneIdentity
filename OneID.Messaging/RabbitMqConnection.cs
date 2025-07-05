using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OneID.Domain.Entities.RabbitSettings;
using OneID.Domain.Interfaces;
using RabbitMQ.Client;

#nullable disable
namespace OneID.Messaging
{
    public sealed class RabbitMqConnection : IRabbitMqConnection
    {
        private IConnection _connection;
        private readonly RabbitMqSettings _rabbitMqSettings;
        private readonly ILogger<RabbitMqConnection> _logger;

        public RabbitMqConnection(IOptions<RabbitMqSettings> rabbitMqSettings, ILogger<RabbitMqConnection> logger)
        {
            _rabbitMqSettings = rabbitMqSettings.Value;
            _logger = logger;
        }

        public async Task InitAsync(IConnectionFactory connectionFactory)
        {
            _connection = await connectionFactory.CreateConnectionAsync();
            _logger.LogInformation("RabbitMQ connection initialized.");
        }

        public async Task<IConnection> GetConnectionAsync()
        {
            const int maxRetries = 3;
            int attempt = 0;

            if (attempt == 0)
            {
                _logger.LogInformation("Delaying initial RabbitMQ connection attempt...");
                await Task.Delay(TimeSpan.FromSeconds(5));
            }

            while (_connection == null || !_connection.IsOpen)
            {
                attempt++;
                if (attempt == 1)
                {
                    _logger.LogInformation("Attempting to establish RabbitMQ connection...");
                }
                else
                {
                    _logger.LogWarning($"Attempt {attempt} of {maxRetries}: Trying to re-establish RabbitMQ connection...");
                }

                try
                {
                    var factory = new ConnectionFactory
                    {
                        HostName = _rabbitMqSettings.HostName,
                        Port = _rabbitMqSettings.Port,
                        UserName = _rabbitMqSettings.UserName,
                        Password = _rabbitMqSettings?.Password,
                        VirtualHost = _rabbitMqSettings.VirtualHost,
                        Ssl = new SslOption
                        {
                            Enabled = true,
                            ServerName = _rabbitMqSettings.HostName,
                            Version = System.Security.Authentication.SslProtocols.Tls12,
                        },
                        AutomaticRecoveryEnabled = true,
                        NetworkRecoveryInterval = TimeSpan.FromSeconds(10),
                        RequestedConnectionTimeout = TimeSpan.FromSeconds(30),
                        RequestedHeartbeat = TimeSpan.FromSeconds(_rabbitMqSettings.Heartbeat > 0 ? _rabbitMqSettings.Heartbeat : 60)

                    };

                    _logger.LogInformation("Initializing RabbitMQ connection with Heartbeat: " +
                        "{Heartbeat} seconds", factory.RequestedHeartbeat.TotalSeconds);

                    _connection = await factory.CreateConnectionAsync();

                    _connection.CallbackExceptionAsync += (sender, ea) =>
                    {
                        _logger.LogInformation($"CallbackException: {ea.Exception}");
                        return Task.CompletedTask;
                    };

                    _logger.LogInformation("RabbitMQ connection is up!");

                }
                catch (Exception ex)
                {
                    _logger.LogError($"Error trying to {(attempt == 1 ? "establish" : "re-establish")} RabbitMQ connection: {ex.Message}");

                    if (attempt >= maxRetries)
                    {
                        _logger.LogWarning("Maximum number of attempts reached. Throwing exception...");
                        throw;
                    }

                    _logger.LogWarning($"Waiting before trying again... ({TimeSpan.FromSeconds(5)} delay)");
                    await Task.Delay(TimeSpan.FromSeconds(5));
                }
            }

            return _connection;
        }

        public bool IsConnected()
        {
            return _connection != null && _connection.IsOpen;
        }

    }
}
