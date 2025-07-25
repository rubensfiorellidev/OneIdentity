﻿using MassTransit;
using Microsoft.Extensions.Logging;
using OneID.Application.Interfaces.Repositories;
using OneID.Application.Messaging.Sagas.Contracts.Events;
using OneID.Domain.Entities.AuditSagas;

namespace OneID.Application.Messaging.Sagas.Consumers
{
    public class AdmissionAuditConsumer : IConsumer<AdmissionAuditRequested>
    {
        private readonly IAdmissionAuditRepository _repository;
        private readonly ILogger<AdmissionAuditConsumer> _logger;

        public AdmissionAuditConsumer(ILogger<AdmissionAuditConsumer> logger,
                                      IAdmissionAuditRepository repository)
        {
            _logger = logger;
            _repository = repository;
        }

        public async Task Consume(ConsumeContext<AdmissionAuditRequested> context)
        {

            try
            {
                var message = context.Message;

                var audit = new AdmissionAudit
                {
                    Id = $"{Ulid.NewUlid()}",
                    CorrelationId = message.CorrelationId,
                    DatabaseId = message.DatabaseId,
                    CurrentState = message.CurrentState,
                    EventName = message.EventName,
                    ProvisioningDate = message.ProvisioningDate,
                    Description = message.Description,
                    Login = message.Login,
                    FirstName = message.FirstName,
                    LastName = message.LastName

                };


                await _repository.AddAsync(audit, context.CancellationToken);

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
