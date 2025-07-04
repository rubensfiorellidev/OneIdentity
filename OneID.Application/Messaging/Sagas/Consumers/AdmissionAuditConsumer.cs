using MassTransit;
using Microsoft.Extensions.Logging;
using OneID.Data.Factories;
using OneID.Domain.Entities;

namespace OneID.Application.Messaging.Sagas.Consumers
{
    public class AdmissionAuditConsumer : IConsumer<AdmissionAudit>
    {
        private readonly IOneDbContextFactory _dbContextFactory;
        private readonly ILogger<AdmissionAuditConsumer> _logger;

        public AdmissionAuditConsumer(ILogger<AdmissionAuditConsumer> logger,
                                                 IOneDbContextFactory dbContextFactory)
        {
            _logger = logger;
            _dbContextFactory = dbContextFactory;
        }

        public async Task Consume(ConsumeContext<AdmissionAudit> context)
        {
            await using var dbContext = _dbContextFactory.CreateDbContext();

            try
            {
                var message = context.Message;

                var audit = new AdmissionAudit
                {
                    Id = $"AUDIT{Ulid.NewUlid()}",
                    CorrelationId = message.CorrelationId,
                    DatabaseId = message.DatabaseId,
                    CurrentState = message.CurrentState,
                    EventName = message.EventName,
                    ProvisioningDate = message.ProvisioningDate,
                    Description = message.Description,
                    Login = message.Login

                };


                await dbContext.AutomaticAdmissionPjAudits.AddAsync(audit, context.CancellationToken);
                await dbContext.SaveChangesAsync(context.CancellationToken);

                _logger.LogInformation(
                    "Audit registrado com sucesso - CorrelationId: {CorrelationId} - CurrentState: {CurrentState}",
                    message.CorrelationId,
                    message.CurrentState);


            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Erro ao registrar auditoria - CorrelationId: {CorrelationId}",
                    context.Message.CorrelationId);

                throw;
            }


        }
    }

}
