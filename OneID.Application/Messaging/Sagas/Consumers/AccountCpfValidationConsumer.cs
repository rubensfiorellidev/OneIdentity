using MassTransit;
using Microsoft.Extensions.Logging;
using OneID.Application.Interfaces.Repositories;
using OneID.Application.Interfaces.Services;
using OneID.Application.Messaging.Sagas.Contracts.Events;
using OneID.Domain.Entities.UserContext;

namespace OneID.Application.Messaging.Sagas.Consumers
{
    public sealed class AccountCpfValidationConsumer : IConsumer<AccountCpfValidationRequested>
    {
        private readonly ILogger<AccountCpfValidationConsumer> _logger;
        private readonly IHashService _hashService;
        private readonly IQueryUserAccountRepository _queryUserAccountRepository;
        private readonly IAdmissionAlertRepository _admissionAlertRepository;
        private readonly IAlertNotifier _alertNotifier;

        public AccountCpfValidationConsumer(
            ILogger<AccountCpfValidationConsumer> logger,
            IHashService hashService,
            IQueryUserAccountRepository queryUserAccountRepository,
            IAdmissionAlertRepository admissionAlertRepository,
            IAlertNotifier alertNotifier)
        {
            _logger = logger;
            _hashService = hashService;
            _queryUserAccountRepository = queryUserAccountRepository;
            _admissionAlertRepository = admissionAlertRepository;
            _alertNotifier = alertNotifier;
        }

        public async Task Consume(ConsumeContext<AccountCpfValidationRequested> context)
        {
            var ct = context.CancellationToken;
            var correlationId = context.Message.CorrelationId;

            try
            {
                _logger.LogInformation("Iniciando validação de CPF - CorrelationId: {CorrelationId}", correlationId);

                var cpfHash = await _hashService.ComputeSha3HashAsync(context.Message.Cpf);

                var existingAccount = await _queryUserAccountRepository.GetByCpfHashAsync(cpfHash, ct);

                if (existingAccount is not null)
                {
                    if (!string.IsNullOrWhiteSpace(existingAccount.Login))
                    {
                        _logger.LogWarning("CPF já cadastrado: {FullName}", existingAccount.FullName);

                        await _admissionAlertRepository.AddAsync(new AdmissionAlert
                        {
                            CpfHash = cpfHash,
                            FullName = context.Message.FullName,
                            PositionHeldId = context.Message.PositionHeldId,
                            WarningMessage = "CPF já cadastrado."

                        }, ct);

                        await _alertNotifier.SendCriticalAlertAsync(
                            subject: "Conflito de CPF em admissão CLT",
                            message:
                                $"CPF duplicado detectado para {context.Message.FullName}.<br>" +
                                $"<strong>CPFHash</strong>: {cpfHash}<br>" +
                                $"<strong>Login existente</strong>: {existingAccount.Login}<br>" +
                                $"<strong>Horário UTC</strong>: {DateTimeOffset.UtcNow:dd/MM/yyyy HH:mm:ss}"
                            );

                        await context.Publish(new AccountCpfValidationFailed
                        {
                            CorrelationId = correlationId,
                            FaultReason = "CPF já cadastrado."
                        });

                        return;
                    }
                    else
                    {
                        _logger.LogError("CPF encontrado sem login: {FullName}", existingAccount.FullName);

                        await _alertNotifier.SendCriticalAlertAsync(
                            subject: "CPF inconsistente",
                            message:
                                $"<strong>CPF {cpfHash}</strong> existe mas está sem login.<br>" +
                                $"<strong>Nome</strong>: {existingAccount.FullName}<br>" +
                                $"<strong>Revisão manual urgente necessária.</strong><br>" +
                                $"<strong>Horário UTC</strong>: {DateTimeOffset.UtcNow:dd/MM/yyyy HH:mm:ss}"
                        );

                        await context.Publish(new AccountCpfValidationFailed
                        {
                            CorrelationId = correlationId,
                            FaultReason = "CPF inconsistente (sem login)."
                        });

                        return;
                    }
                }

                _logger.LogInformation("CPF validado com sucesso - CorrelationId: {CorrelationId}", correlationId);

                await context.Publish(new AccountCpfValidated
                {
                    CorrelationId = correlationId
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro inesperado na validação de CPF - CorrelationId: {CorrelationId}", correlationId);

                await context.Publish(new AccountCpfValidationFailed
                {
                    CorrelationId = correlationId,
                    FaultReason = "Erro interno ao validar CPF"
                });

                throw;
            }
        }
    }

}
