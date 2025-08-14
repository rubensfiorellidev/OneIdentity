using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Npgsql;
using OneID.Application.Interfaces.Logins;
using OneID.Application.Messaging.Sagas.Consumers;
using OneID.Application.Messaging.Sagas.Contracts;
using OneID.Application.Messaging.Sagas.StatesMachines;
using OneID.Data.DataContexts;
using OneID.Data.Repositories.Logins;
using OneID.Domain.Entities.ApiOptions;
using OneID.Domain.Entities.RabbitSettings;
using RabbitMQ.Client;
using System.Net.Security;
using System.Security.Authentication;

#nullable disable
namespace OneID.Messaging
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddMassTrasitConfig(this IServiceCollection services)
        {
            // Infra de login/idempotência
            services.AddScoped<ILoginReservationRepository, LoginReservationRepository>();
            services.AddScoped<IIdempotencyStore, IdempotencyStore>();

            services.AddMassTransit(x =>
            {
                // SAGA
                x.AddSagaStateMachine<AccountStateMachine, AccountSagaState>()
                    .EntityFrameworkRepository(r =>
                    {
                        r.ConcurrencyMode = ConcurrencyMode.Optimistic;
                        r.AddDbContext<DbContext, OneDbContext>((provider, builder) =>
                        {
                            var env = provider.GetRequiredService<IHostEnvironment>();
                            var config = provider.GetRequiredService<IConfiguration>();

                            var cs = env.IsDevelopment()
                                ? config.GetConnectionString("NPSqlConnection")
                                : env.IsEnvironment("Staging")
                                    ? config.GetConnectionString("NPSqlConnectionStaging")
                                    : config.GetConnectionString("NPSqlConnectionProduction");

                            builder.UseNpgsql(cs, npgsql =>
                            {
                                npgsql.MigrationsAssembly(typeof(OneDbContext).Assembly.FullName);
                                npgsql.EnableRetryOnFailure(5, TimeSpan.FromSeconds(30), null);
                            });
                        });
                    });

                // Consumers (inclui compensações)
                x.AddConsumer<CreateLoginConsumer>();
                x.AddConsumer<AdmissionAuditConsumer>();
                x.AddConsumer<KeycloakUserProvisioningConsumer>();
                x.AddConsumer<AccountCpfValidationConsumer>();
                x.AddConsumer<CreateAccountDatabaseConsumer>();
                x.AddConsumer<CommitLoginConsumer>();
                x.AddConsumer<ReleaseLoginConsumer>();

                // (Opcional, recomendado) EF Outbox para consistência publish==commit
                x.AddEntityFrameworkOutbox<OneDbContext>(o =>
                {
                    o.UsePostgres();
                    o.QueryDelay = TimeSpan.FromSeconds(1);
                    o.DuplicateDetectionWindow = TimeSpan.FromMinutes(10);
                });

                x.UsingRabbitMq((context, cfg) =>
                {
                    var logger = context.GetRequiredService<ILoggerFactory>().CreateLogger("MassTransitSetup");
                    var config = context.GetRequiredService<IConfiguration>();
                    var rabbit = config.GetSection("RabbitConnection").Get<RabbitConnectionSettings>();

                    string NormalizeVirtualHost(string vhost) => string.IsNullOrWhiteSpace(vhost) ? "/" : vhost.Trim('/');
                    var virtualHost = NormalizeVirtualHost(rabbit.VirtualHost);

                    cfg.Host(new Uri($"rabbitmq://{rabbit.HostName}:{rabbit.Port}/{virtualHost}"), h =>
                    {
                        h.Username(rabbit.Username);
                        h.Password(rabbit.Password);
                        h.UseSsl(s =>
                        {
                            s.Protocol = SslProtocols.Tls12;
                            s.ServerName = rabbit.HostName;
                            s.AllowPolicyErrors(SslPolicyErrors.None);
                        });
                    });

                    // Middlewares globais
                    cfg.UseDelayedRedelivery(r => r.Interval(3, TimeSpan.FromSeconds(10)));
                    cfg.UseMessageRetry(r => r.Interval(3, TimeSpan.FromSeconds(5)));
                    cfg.UseCircuitBreaker(cb =>
                    {
                        cb.TrackingPeriod = TimeSpan.FromMinutes(1);
                        cb.TripThreshold = 5;
                        cb.ActiveThreshold = 10;
                        cb.ResetInterval = TimeSpan.FromMinutes(5);
                    });
                    cfg.UseKillSwitch(k =>
                    {
                        k.SetActivationThreshold(10);
                        k.SetTripThreshold(0.20);
                        k.SetRestartTimeout(TimeSpan.FromMinutes(1));
                    });

                    // --- Endpoints ---

                    cfg.ReceiveEndpoint("create-account-saga", e =>
                    {
                        e.PrefetchCount = 10;
                        e.UseInMemoryOutbox(context);
                        e.UseMessageRetry(r =>
                        {
                            r.Handle<TimeoutException>();
                            r.Handle<HttpRequestException>();
                            r.Handle<NpgsqlException>(ex => ex.IsTransient);
                            r.Handle<DbUpdateException>(ex => ex.InnerException is NpgsqlException npg && npg.IsTransient);
                            r.Interval(3, TimeSpan.FromSeconds(10));
                        });

                        e.ConfigureSaga<AccountSagaState>(context);
                        logger.LogInformation("Fila [create-account-saga] registrada com sucesso.");
                    });

                    cfg.ReceiveEndpoint("create-login", e =>
                    {
                        e.PrefetchCount = 10;
                        e.UseInMemoryOutbox(context);
                        e.UseMessageRetry(r =>
                        {
                            r.Interval(3, TimeSpan.FromSeconds(5));
                            r.Handle<TimeoutException>();
                            r.Handle<HttpRequestException>();
                            r.Handle<NpgsqlException>(ex => ex.IsTransient);
                            r.Handle<DbUpdateException>(ex => ex.InnerException is NpgsqlException npg && npg.IsTransient);
                        });
                        e.ConfigureConsumer<CreateLoginConsumer>(context);
                        logger.LogInformation("Fila [create-login] registrada com sucesso.");
                    });

                    cfg.ReceiveEndpoint("admission-audit", e =>
                    {
                        e.PrefetchCount = 10;
                        e.UseInMemoryOutbox(context);
                        e.UseMessageRetry(r =>
                        {
                            r.Handle<TimeoutException>();
                            r.Handle<HttpRequestException>();
                            r.Handle<NpgsqlException>(ex => ex.IsTransient);
                            r.Handle<DbUpdateException>(ex => ex.InnerException is NpgsqlException npg && npg.IsTransient);
                            r.Interval(3, TimeSpan.FromSeconds(10));
                        });
                        e.ConfigureConsumer<AdmissionAuditConsumer>(context);
                        logger.LogInformation("Fila [admission-audit] registrada com sucesso.");
                    });

                    cfg.ReceiveEndpoint("keycloak-provisioning", e =>
                    {
                        e.PrefetchCount = 10;
                        e.UseInMemoryOutbox(context);
                        e.UseMessageRetry(r =>
                        {
                            r.Handle<TimeoutException>();
                            r.Handle<HttpRequestException>();
                            r.Handle<NpgsqlException>(ex => ex.IsTransient);
                            r.Handle<DbUpdateException>(ex => ex.InnerException is NpgsqlException npg && npg.IsTransient);
                            r.Interval(3, TimeSpan.FromSeconds(10));
                        });
                        e.ConfigureConsumer<KeycloakUserProvisioningConsumer>(context);
                        logger.LogInformation("Fila [keycloak-provisioning] registrada com sucesso.");
                    });

                    cfg.ReceiveEndpoint("cpf-validation", e =>
                    {
                        e.PrefetchCount = 10;
                        e.UseInMemoryOutbox(context);
                        e.UseMessageRetry(r =>
                        {
                            r.Interval(3, TimeSpan.FromSeconds(5));
                            r.Handle<TimeoutException>();
                            r.Handle<HttpRequestException>();
                            r.Handle<NpgsqlException>(ex => ex.IsTransient);
                            r.Handle<DbUpdateException>(ex => ex.InnerException is NpgsqlException npg && npg.IsTransient);
                        });
                        e.ConfigureConsumer<AccountCpfValidationConsumer>(context);
                        logger.LogInformation("Fila [cpf-validation] registrada com sucesso.");
                    });

                    cfg.ReceiveEndpoint("create-account-database", e =>
                    {
                        e.PrefetchCount = 10;
                        e.UseInMemoryOutbox(context);
                        e.UseMessageRetry(r =>
                        {
                            r.Interval(3, TimeSpan.FromSeconds(5));
                            r.Handle<TimeoutException>();
                            r.Handle<HttpRequestException>();
                            r.Handle<NpgsqlException>(ex => ex.IsTransient);
                            r.Handle<DbUpdateException>(ex => ex.InnerException is NpgsqlException npg && npg.IsTransient);
                        });
                        e.ConfigureConsumer<CreateAccountDatabaseConsumer>(context);
                        logger.LogInformation("Fila [create-account-database] registrada com sucesso.");
                    });

                    cfg.ReceiveEndpoint("commit-login", e =>
                    {
                        e.PrefetchCount = 10;
                        e.UseInMemoryOutbox(context);
                        e.UseMessageRetry(r => r.Incremental(3, TimeSpan.FromSeconds(2), TimeSpan.FromSeconds(5)));
                        e.ConfigureConsumer<CommitLoginConsumer>(context);
                        logger.LogInformation("Fila [commit-login] registrada com sucesso.");
                    });

                    cfg.ReceiveEndpoint("release-login", e =>
                    {
                        e.PrefetchCount = 10;
                        e.UseInMemoryOutbox(context);
                        e.UseMessageRetry(r => r.Incremental(3, TimeSpan.FromSeconds(2), TimeSpan.FromSeconds(5)));
                        e.ConfigureConsumer<ReleaseLoginConsumer>(context);
                        logger.LogInformation("Fila [release-login] registrada com sucesso.");
                    });
                });
            });

            return services;
        }

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


    }
}
