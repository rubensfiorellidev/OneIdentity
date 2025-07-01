using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace OneID.Messaging
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddMessaging(this IServiceCollection services, IConfiguration configuration)
        {
            // Ex: services.AddSingleton<IMessagePublisher, RabbitMqPublisher>();
            return services;
        }
    }
}
