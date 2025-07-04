using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace OneID.Shared
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddMassTransitSetup(this IServiceCollection services, IConfiguration configuration, IHostEnvironment environment)
        {

            //services.AddMassTransit(x =>
            //{
            //    x.AddSagaStateMachine<AccountStateMachine, AccountSagaState>()
            //        .EntityFrameworkRepository(r =>
            //        {
            //            r.ConcurrencyMode = ConcurrencyMode.Optimistic;
            //            r.AddDbContext<DbContext, OneIdDbContext>((provider, builder) =>
            //            {
            //                var env = provider.GetRequiredService<IHostEnvironment>();
            //                var config = provider.GetRequiredService<IConfiguration>();

            //                var connectionString = env.IsDevelopment()
            //                    ? config.GetConnectionString("NPSqlConnectionDev")
            //                    : env.IsEnvironment("Staging")
            //                        ? config.GetConnectionString("NPSqlConnectionQa")
            //                        : config.GetConnectionString("NPSqlConnectionPrd");

            //                builder.UseNpgsql(connectionString, npgsql =>
            //                {
            //                    npgsql.MigrationsAssembly(typeof(OneIdDbContext).Assembly.FullName);
            //                    npgsql.EnableRetryOnFailure(5, TimeSpan.FromSeconds(30), null);
            //                });
            //            });
            //        });


            //    x.AddConsumer<CreateAccountPjDatabaseConsumer>();
            //    x.AddConsumer<CreateLoginForPjConsumer>();
            //    x.AddConsumer<AutomaticAdmissionPjAuditConsumer>();
            //    x.AddConsumer<AccountPjCpfValidationConsumer>();
            //    x.AddConsumer<AccountPjKitValidationConsumer>();
            //    x.AddConsumer<CreateOutboxForAccountPjConsumer>();
            //    x.AddConsumer<MarkOutboxHandlerProcessedConsumer>();
            //    x.AddConsumer<SendTermsEmailForAccountPjConsumer>();
            //    x.AddConsumer<AccountPjDynamicGroupAssignedConsumer>();
            //    x.AddConsumer<AccountPjM365LicenseProvisioningConsumer>();




            //    x.AddConsumer<CreateAccountCltCommandConsumer>();
            //    x.AddConsumer<AutomaticAdmissionCltAuditConsumer>();

            //    x.AddConsumer<AutomaticAdmissionPjAuditConsumer>()
            //        .Endpoint(e => e.Name = "automatic-admission-pj-audit");

            //    x.UsingRabbitMq((context, cfg) =>
            //    {
            //        var logger = context.GetRequiredService<ILoggerFactory>().CreateLogger("MassTransitSetup");
            //        var config = context.GetRequiredService<IConfiguration>();
            //        var rabbit = config.GetSection("RabbitConnection").Get<RabbitConnectionSettings>();

            //        string NormalizeVirtualHost(string vhost) => string.IsNullOrWhiteSpace(vhost) ? "/" : vhost.Trim('/');

            //        var virtualHost = NormalizeVirtualHost(rabbit.VirtualHost);

            //        cfg.Host(new Uri($"rabbitmq://{rabbit.HostName}:{rabbit.Port}/{virtualHost}"), h =>
            //        {
            //            h.Username(rabbit.UserName);
            //            h.Password(rabbit.Password);
            //            h.UseSsl(s =>
            //            {
            //                s.Protocol = SslProtocols.Tls12;
            //                s.ServerName = rabbit.HostName;
            //                s.AllowPolicyErrors(SslPolicyErrors.None);
            //            });
            //        });

            //        cfg.UseDelayedRedelivery(r => r.Interval(3, TimeSpan.FromSeconds(10)));
            //        cfg.UseMessageRetry(r => r.Interval(3, TimeSpan.FromSeconds(5)));

            //        cfg.UseCircuitBreaker(cb =>
            //        {
            //            cb.TrackingPeriod = TimeSpan.FromMinutes(1);
            //            cb.TripThreshold = 5;
            //            cb.ActiveThreshold = 10;
            //            cb.ResetInterval = TimeSpan.FromMinutes(5);
            //        });

            //        cfg.UseKillSwitch(k =>
            //        {
            //            k.SetActivationThreshold(10);
            //            k.SetTripThreshold(0.20);
            //            k.SetRestartTimeout(TimeSpan.FromMinutes(1));
            //        });

            //        cfg.ReceiveEndpoint("create-account-clt-saga", e =>
            //        {
            //            e.PrefetchCount = 10;
            //            e.UseInMemoryOutbox(context);

            //            e.UseMessageRetry(r => r.Interval(3, TimeSpan.FromSeconds(5)));

            //            e.ConfigureSaga<AccountSagaState>(context);

            //            logger.LogInformation("Fila [create-account-clt-saga] registrada com sucesso.");
            //        });

            //        cfg.ReceiveEndpoint("create-account-clt-db", e =>
            //        {
            //            e.PrefetchCount = 10;
            //            e.UseInMemoryOutbox(context);

            //            e.ConfigureConsumer<CreateAccountCltCommandConsumer>(context);

            //            logger.LogInformation("Fila [create-account-clt-db] registrada com sucesso.");
            //        });

            //        cfg.ReceiveEndpoint("automatic-admission-clt-audit", e =>
            //        {
            //            e.PrefetchCount = 10;
            //            e.UseInMemoryOutbox(context);

            //            e.ConfigureConsumer<AutomaticAdmissionCltAuditConsumer>(context);

            //            logger.LogInformation("Fila [automatic-admission-clt-audit] registrada com sucesso.");
            //        });

            //        cfg.ReceiveEndpoint("create-account-pj-saga", e =>
            //        {
            //            e.PrefetchCount = 10;
            //            e.UseInMemoryOutbox(context);
            //            e.UseMessageRetry(r =>
            //            {
            //                r.Interval(5, TimeSpan.FromSeconds(1)); // 5 tentativas, 1 segundo de intervalo
            //                r.Handle<DbUpdateConcurrencyException>(); // Tratar conflitos de concorrência
            //            });
            //            e.ConfigureSaga<AccountPjSagaState>(context);
            //            logger.LogInformation("Fila [create-account-pj-saga] registrada com sucesso.");
            //        });

            //        cfg.ReceiveEndpoint("create-login-pj", e =>
            //        {
            //            e.PrefetchCount = 10;
            //            e.UseInMemoryOutbox(context);

            //            e.UseMessageRetry(r => r.Interval(3, TimeSpan.FromSeconds(5)));
            //            e.ConfigureConsumer<CreateLoginForPjConsumer>(context);

            //            logger.LogInformation("Fila [create-login-pj] registrada com sucesso.");
            //        });

            //        cfg.ReceiveEndpoint("create-account-pj-db", e =>
            //        {
            //            e.PrefetchCount = 10;
            //            e.UseInMemoryOutbox(context);

            //            e.UseMessageRetry(r =>
            //            {
            //                r.Handle<TimeoutException>();
            //                r.Handle<HttpRequestException>();
            //                r.Handle<NpgsqlException>(ex => ex.IsTransient);
            //                r.Handle<DbUpdateException>(ex => ex.InnerException is NpgsqlException npg && npg.IsTransient);
            //                r.Interval(3, TimeSpan.FromSeconds(10));
            //            });

            //            e.ConfigureConsumer<CreateAccountPjDatabaseConsumer>(context);

            //            logger.LogInformation("Fila [create-account-pj-db] registrada com sucesso.");
            //        });

            //        cfg.ReceiveEndpoint("automatic-admission-pj-audit", e =>
            //        {
            //            e.PrefetchCount = 10;
            //            e.UseInMemoryOutbox(context);

            //            e.UseMessageRetry(r =>
            //            {
            //                r.Handle<TimeoutException>();
            //                r.Handle<NpgsqlException>(ex => ex.IsTransient);
            //                r.Interval(3, TimeSpan.FromSeconds(10));
            //            });

            //            e.ConfigureConsumer<AutomaticAdmissionPjAuditConsumer>(context);

            //            logger.LogInformation("Fila [automatic-admission-pj-audit] registrada com sucesso.");
            //        });

            //        cfg.ReceiveEndpoint("validate-account-pj-cpf", e =>
            //        {
            //            e.PrefetchCount = 10;
            //            e.UseInMemoryOutbox(context);

            //            e.UseMessageRetry(r =>
            //            {
            //                r.Handle<TimeoutException>();
            //                r.Handle<NpgsqlException>(ex => ex.IsTransient);
            //                r.Handle<DbUpdateException>(ex => ex.InnerException is NpgsqlException npg && npg.IsTransient);
            //                r.Interval(3, TimeSpan.FromSeconds(10));
            //            });

            //            e.ConfigureConsumer<AccountPjCpfValidationConsumer>(context);

            //            logger.LogInformation("Fila [validate-account-pj-cpf] registrada com sucesso.");
            //        });

            //        cfg.ReceiveEndpoint("account-pj-kit-validation", e =>
            //        {
            //            e.PrefetchCount = 10;
            //            e.UseInMemoryOutbox(context);
            //            e.UseMessageRetry(r => r.Interval(3, TimeSpan.FromSeconds(5)));

            //            e.ConfigureConsumer<AccountPjKitValidationConsumer>(context);

            //            logger.LogInformation("Fila [account-pj-kit-validation] registrada com sucesso.");
            //        });

            //        cfg.ReceiveEndpoint("create-outbox-account-pj", e =>
            //        {
            //            e.PrefetchCount = 10;
            //            e.UseInMemoryOutbox(context);

            //            e.UseMessageRetry(r =>
            //            {
            //                r.Handle<TimeoutException>();
            //                r.Handle<NpgsqlException>(ex => ex.IsTransient);
            //                r.Handle<DbUpdateException>(ex => ex.InnerException is NpgsqlException npg && npg.IsTransient);
            //                r.Interval(3, TimeSpan.FromSeconds(10));
            //            });

            //            e.ConfigureConsumer<CreateOutboxForAccountPjConsumer>(context);

            //            logger.LogInformation("Fila [create-outbox-account-pj] registrada com sucesso.");
            //        });

            //        cfg.ReceiveEndpoint("account-pj-terms-email-send-requested", e =>
            //        {
            //            e.PrefetchCount = 10;
            //            e.UseInMemoryOutbox(context);

            //            e.UseMessageRetry(r =>
            //            {
            //                r.Handle<TimeoutException>();
            //                r.Handle<NpgsqlException>(ex => ex.IsTransient);
            //                r.Handle<DbUpdateException>(ex => ex.InnerException is NpgsqlException npg && npg.IsTransient);
            //                r.Interval(3, TimeSpan.FromSeconds(10));
            //            });

            //            e.ConfigureConsumer<SendTermsEmailForAccountPjConsumer>(context);

            //            logger.LogInformation("Fila [account-pj-terms-email-send-requested] registrada com sucesso.");
            //        });

            //        cfg.ReceiveEndpoint("mark-outbox-handler-processed", e =>
            //        {
            //            e.PrefetchCount = 10;
            //            e.UseInMemoryOutbox(context);

            //            e.ConfigureConsumer<MarkOutboxHandlerProcessedConsumer>(context);

            //            logger.LogInformation("Fila [mark-outbox-handler-processed] registrada com sucesso.");
            //        });

            //        cfg.ReceiveEndpoint("account-pj-dynamic-group-assigned", e =>
            //        {
            //            e.PrefetchCount = 10;
            //            e.UseInMemoryOutbox(context);
            //            e.ConfigureConsumer<AccountPjDynamicGroupAssignedConsumer>(context);

            //            logger.LogInformation("Fila [account-pj-dynamic-group-assigned] registrada com sucesso.");
            //        });

            //        cfg.ReceiveEndpoint("account-pj-m365-license-delayed", e =>
            //        {
            //            e.PrefetchCount = 10;
            //            e.UseInMemoryOutbox(context);

            //            e.ConfigureConsumeTopology = false;
            //            e.SetQueueArgument("x-dead-letter-exchange", "account-pj-m365-license-exchange");
            //            e.SetQueueArgument("x-message-ttl", 30 * 60 * 1000); // 30 minutos

            //            logger.LogInformation("Fila [account-pj-m365-license-delayed] registrada com sucesso.");
            //        });

            //        cfg.ReceiveEndpoint("account-pj-m365-license", e =>
            //        {
            //            e.PrefetchCount = 10;
            //            e.UseInMemoryOutbox(context);

            //            e.ConfigureConsumer<AccountPjM365LicenseProvisioningConsumer>(context);

            //            logger.LogInformation("Fila [account-pj-m365-license] registrada com sucesso.");
            //        });

            //        cfg.Message<AccountPjM365LicenseProvisioningRequested>(x =>
            //        {
            //            x.SetEntityName("account-pj-m365-license-exchange"); // exchange principal
            //        });


            //    });
            //});


            return services;
        }

    }
}
