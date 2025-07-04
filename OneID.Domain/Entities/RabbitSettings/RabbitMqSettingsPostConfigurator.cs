using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace OneID.Domain.Entities.RabbitSettings
{
    public sealed class RabbitMqSettingsPostConfigurator : IPostConfigureOptions<RabbitMqSettings>
    {
        private readonly IConfiguration _configuration;
        public RabbitMqSettingsPostConfigurator(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public void PostConfigure(string? name, RabbitMqSettings options)
        {
            var connectionSection = _configuration.GetSection("RabbitConnection");
            if (connectionSection.Exists())
            {
                options.HostName = connectionSection["Hostname"];
                options.Port = int.TryParse(connectionSection["Port"], out var port) ? port : 5671;
                options.Username = connectionSection["Username"];
                options.Password = connectionSection["Password"];
                options.VirtualHost = connectionSection["VirtualHost"];
            }
        }
    }
}
