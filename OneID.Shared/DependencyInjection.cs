using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using OneID.Application.Abstractions;
using OneID.Domain.Entities.ApiOptions;
using OneID.Domain.Entities.RabbitSettings;
using RabbitMQ.Client;

#nullable disable
namespace OneID.Shared
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddRabbitSetup(this IServiceCollection services, IConfiguration configuration, IHostEnvironment environment)
        {
            services.AddSingleton<IPostConfigureOptions<RabbitMqSettings>, RabbitMqSettingsPostConfigurator>();

            services.Configure<RabbitMqSettings>(configuration.GetSection("RabbitMqSettings"))
                    .Configure<RabbitMqUrlSettings>(configuration.GetSection("RabbitMqUrlSettings"))
                    .Configure<SafeBindingsSettings>(configuration.GetSection("RabbitMqSettings:SafeBindings"));



            services.Configure<ApiUrlSettings>(options =>
            {
                var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

                if (string.IsNullOrEmpty(environment))
                {
                    throw new InvalidOperationException("ASPNETCORE_ENVIRONMENT is not set.");
                }

                var configSection = environment switch
                {
                    "Development" => "ApiOptions:Development",
                    "Staging" => "ApiOptions:Staging",
                    "Production" => "ApiOptions:Production",
                    _ => throw new InvalidOperationException($"Unknown environment: {environment}")
                };

                var environmentConfig = configuration.GetSection(configSection);
                environmentConfig.Bind(options);
            });

            services.AddSingleton<IConnectionFactory>(sp =>
            {
                var rabbitMqSettings = sp.GetRequiredService<IOptions<RabbitMqSettings>>().Value;

                return new ConnectionFactory
                {
                    HostName = rabbitMqSettings.HostName,
                    Port = rabbitMqSettings.Port,
                    UserName = rabbitMqSettings.UserName,
                    Password = rabbitMqSettings.Password,
                    VirtualHost = rabbitMqSettings.VirtualHost,
                    Ssl = new SslOption
                    {
                        Enabled = true,
                        ServerName = rabbitMqSettings.HostName
                    },
                    AutomaticRecoveryEnabled = true,
                    NetworkRecoveryInterval = TimeSpan.FromSeconds(10)
                };
            });

            services.AddSingleton<RabbitMqConnection>();



            return services;
        }

        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration, IHostEnvironment environment)
        {

            services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssemblies(AppDomain.CurrentDomain.GetAssemblies());
                cfg.AddOpenBehavior(typeof(RequestLoggingPipelineBehavior<,>));
            });


            return services;
        }
    }
}
