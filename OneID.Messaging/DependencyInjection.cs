using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Npgsql;
using OneID.Application.Messaging.Sagas.Consumers;
using OneID.Application.Messaging.Sagas.Contracts;
using OneID.Application.Messaging.Sagas.StatesMachines;
using OneID.Data.DataContexts;
using OneID.Domain.Entities.RabbitSettings;
using System.Net.Security;
using System.Security.Authentication;

#nullable disable
namespace OneID.Messaging
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddMessaging(this IServiceCollection services)
        {
            services.AddMassTransit(x =>
            {
                x.AddSagaStateMachine<AccountStateMachine, AccountSagaState>()
                    .EntityFrameworkRepository(r =>
                    {
                        r.ConcurrencyMode = ConcurrencyMode.Optimistic;
                        r.AddDbContext<DbContext, OneDbContext>((provider, builder) =>
                        {
                            var env = provider.GetRequiredService<IHostEnvironment>();
                            var config = provider.GetRequiredService<IConfiguration>();

                            var connectionString = env.IsDevelopment()
                                ? config.GetConnectionString("NPSqlConnection")
                                : env.IsEnvironment("Staging")
                                    ? config.GetConnectionString("NPSqlConnectionStaging")
                                    : config.GetConnectionString("NPSqlConnectionProduction");

                            builder.UseNpgsql(connectionString, npgsql =>
                            {
                                npgsql.MigrationsAssembly(typeof(OneDbContext).Assembly.FullName);
                                npgsql.EnableRetryOnFailure(5, TimeSpan.FromSeconds(30), null);
                            });
                        });
                    });


                //x.AddConsumer<CreateAccountPjDatabaseConsumer>();
                x.AddConsumer<CreateLoginConsumer>();
                x.AddConsumer<AdmissionAuditConsumer>();


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

                    cfg.ReceiveEndpoint("create-account-saga", e =>
                    {
                        e.PrefetchCount = 10;
                        e.UseInMemoryOutbox(context);
                        e.UseMessageRetry(r =>
                        {
                            r.Interval(5, TimeSpan.FromSeconds(1)); // 5 tentativas, 1 segundo de intervalo
                            r.Handle<DbUpdateConcurrencyException>(); // Tratar conflitos de concorrência
                        });
                        e.ConfigureSaga<AccountSagaState>(context);
                        logger.LogInformation("Fila [create-account-saga] registrada com sucesso.");
                    });

                    cfg.ReceiveEndpoint("create-login", e =>
                    {
                        e.PrefetchCount = 10;
                        e.UseInMemoryOutbox(context);

                        e.UseMessageRetry(r => r.Interval(3, TimeSpan.FromSeconds(5)));
                        e.ConfigureConsumer<CreateLoginConsumer>(context);

                        logger.LogInformation("Fila [create-login] registrada com sucesso.");
                    });

                    //cfg.ReceiveEndpoint("create-account-pj-db", e =>
                    //{
                    //    e.PrefetchCount = 10;
                    //    e.UseInMemoryOutbox(context);

                    //    e.UseMessageRetry(r =>
                    //    {
                    //        r.Handle<TimeoutException>();
                    //        r.Handle<HttpRequestException>();
                    //        r.Handle<NpgsqlException>(ex => ex.IsTransient);
                    //        r.Handle<DbUpdateException>(ex => ex.InnerException is NpgsqlException npg && npg.IsTransient);
                    //        r.Interval(3, TimeSpan.FromSeconds(10));
                    //    });

                    //    e.ConfigureConsumer<CreateAccountPjDatabaseConsumer>(context);

                    //    logger.LogInformation("Fila [create-account-pj-db] registrada com sucesso.");
                    //});

                    cfg.ReceiveEndpoint("admission-audit", e =>
                    {
                        e.PrefetchCount = 10;
                        e.UseInMemoryOutbox(context);

                        e.UseMessageRetry(r =>
                        {
                            r.Handle<TimeoutException>();
                            r.Handle<NpgsqlException>(ex => ex.IsTransient);
                            r.Interval(3, TimeSpan.FromSeconds(10));
                        });

                        e.ConfigureConsumer<AdmissionAuditConsumer>(context);

                        logger.LogInformation("Fila [admission-audit] registrada com sucesso.");
                    });

                });
            });

            return services;
        }



    }
}
