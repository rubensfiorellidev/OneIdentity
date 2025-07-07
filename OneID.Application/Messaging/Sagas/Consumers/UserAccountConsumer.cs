using MassTransit;
using Microsoft.Extensions.Logging;
using OneID.Application.Commands;
using OneID.Application.Interfaces.CQRS;
using OneID.Application.Interfaces.Services;
using OneID.Application.Messaging.Sagas.Contracts.Events;

namespace OneID.Application.Messaging.Sagas.Consumers
{
    public class UserAccountConsumer : IConsumer<CreateUserAccountRequested>
    {
        private readonly ICommandHandler<CreateUserAccountCommand, IResult> _handler;
        private readonly ILogger<UserAccountConsumer> _logger;

        public UserAccountConsumer(
            ICommandHandler<CreateUserAccountCommand, IResult> handler,
            ILogger<UserAccountConsumer> logger)
        {
            _handler = handler ?? throw new ArgumentNullException(nameof(handler));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task Consume(ConsumeContext<CreateUserAccountRequested> context)
        {
            try
            {
                var message = context.Message;

                var command = new CreateUserAccountCommand
                {
                    FullName = message.DatabasePayload.FullName,
                    SocialName = message.DatabasePayload.SocialName,
                    Cpf = message.DatabasePayload.Cpf,
                    BirthDate = message.DatabasePayload.BirthDate,
                    DateOfHire = message.DatabasePayload.DateOfHire,
                    Registry = message.DatabasePayload.Registry,
                    MotherName = message.DatabasePayload.MotherName,
                    Company = message.DatabasePayload.Company,
                    Login = message.DatabasePayload.Login,
                    CorporateEmail = message.DatabasePayload.CorporateEmail,
                    PersonalEmail = message.DatabasePayload.PersonalEmail,
                    StatusUserProfile = message.DatabasePayload.StatusUserAccount,
                    TypeUserProfile = message.DatabasePayload.TypeUserAccount,
                    LoginManager = message.DatabasePayload.LoginManager,
                    PositionHeldId = message.DatabasePayload.PositionHeldId,
                    FiscalNumberIdentity = message.DatabasePayload.FiscalNumberIdentity,
                    ContractorCnpj = message.DatabasePayload.ContractorCnpj,
                    ContractorName = message.DatabasePayload.ContractorName,
                    CreatedBy = message.DatabasePayload.CreatedBy
                };


                var result = await _handler.Handle(command, context.CancellationToken).ConfigureAwait(false);

                if (result.IsSuccess)
                {
                    await context.Publish(new UserProfilePersisted
                    {
                        CorrelationId = context.Message.CorrelationId,
                        DatabaseId = result.Data?.ToString() ?? "NotAvailable"
                    });

                }
                else
                {
                    await context.Publish(new UserProfilePersistenceFailed
                    {
                        CorrelationId = message.CorrelationId,
                        FaultReason = string.Join("; ", result.Errors)
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro inesperado ao processar UserProfilePersistenceRequested");
                await context.Publish(new UserProfilePersistenceFailed
                {
                    CorrelationId = context.Message.CorrelationId,
                    FaultReason = ex.Message
                });
            }
        }
    }

}
