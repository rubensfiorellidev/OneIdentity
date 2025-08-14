using MassTransit;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using OneID.Application.DTOs.Logins;
using OneID.Application.Interfaces.Logins;
using OneID.Application.Interfaces.Services;
using OneID.Application.Messaging.Sagas.Contracts.Events;

#nullable disable
namespace OneID.Application.Messaging.Sagas.Consumers
{
    public sealed class CreateLoginConsumer : IConsumer<CreateLoginRequested>
    {
        private readonly ILogger<CreateLoginConsumer> _logger;
        private readonly IUserLoginGenerator _loginGenerator;
        private readonly ILoginReservationRepository _loginRepo;
        private readonly IIdempotencyStore _idempo;


        public CreateLoginConsumer(ILogger<CreateLoginConsumer> logger, IUserLoginGenerator loginGenerator,
                                   ILoginReservationRepository loginRepo, IIdempotencyStore idempo)
        {
            _logger = logger;
            _loginGenerator = loginGenerator;
            _loginRepo = loginRepo;
            _idempo = idempo;
        }

        public async Task Consume(ConsumeContext<CreateLoginRequested> context)
        {
            var message = context.Message;
            var ct = context.CancellationToken;
            string reservedLogin = null;

            var idempoKey = $"idempo:{message.CorrelationId}:{nameof(CreateLoginRequested)}";
            var (found, payload) = await _idempo.TryGetAsync(idempoKey, ct);
            if (found && payload is not null)
            {
                var data = JsonConvert.DeserializeObject<LoginPayload>(payload);

                _logger.LogInformation(
                    "Idempotência: reemitindo LoginCreated {Login} - CorrelationId: {CorrelationId}",
                    data.Login, message.CorrelationId);

                await context.Publish(new LoginCreated
                {
                    CorrelationId = message.CorrelationId,
                    Login = data.Login,
                    CorporateEmail = data.CorporateEmail
                });

                return;
            }

            try
            {
                _logger.LogInformation(
                    "Iniciando provisionamento de login para {Firstname} {Lastname} - CorrelationId: {CorrelationId}",
                    message.FirstName, message.LastName, message.CorrelationId);

                var fullName = $"{message.FirstName} {message.LastName}";
                var suggested = await _loginGenerator.GenerateLoginAsync(fullName, ct);

                reservedLogin = await _loginRepo.ReserveAsync(suggested, message.CorrelationId, ct);
                var corporateEmail = $"{reservedLogin}@oneidsecure.cloud";

                _logger.LogInformation(
                    "Login {Login} reservado com sucesso - CorrelationId: {CorrelationId}",
                    reservedLogin, message.CorrelationId);

                await _idempo.MarkAsync(idempoKey, new LoginPayload(reservedLogin, corporateEmail), ct);

                await context.Publish(new LoginCreated
                {
                    CorrelationId = message.CorrelationId,
                    Login = reservedLogin,
                    CorporateEmail = corporateEmail
                });
            }
            catch (Exception ex)
            {
                if (reservedLogin is not null)
                    await _loginRepo.ReleaseAsync(message.CorrelationId, ct);

                _logger.LogError(ex,
               "Erro ao provisionar login - CorrelationId: {CorrelationId}",
               message.CorrelationId);

                await context.Publish(new LoginFailed
                {
                    CorrelationId = message.CorrelationId,
                    FaultReason = ex.Message
                });

                throw;
            }
        }
    }
}
