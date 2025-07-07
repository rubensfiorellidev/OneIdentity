using MassTransit;
using Microsoft.Extensions.Logging;
using OneID.Application.Commands;
using OneID.Application.Interfaces.CQRS;
using OneID.Application.Messaging.Sagas.Contracts.Events;

namespace OneID.Application.Messaging.Sagas.Consumers
{
    public class UserAccountConsumer : IConsumer<UserAccountPersistenceRequested>
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

        public async Task Consume(ConsumeContext<UserAccountPersistenceRequested> context)
        {
            try
            {
                var message = context.Message;

                var command = new CreateUserAccountCommand
                {
                    FullName = message.Payload.FullName,
                    SocialName = message.Payload.SocialName,
                    Cpf = message.Payload.Cpf,
                    BirthDate = message.Payload.BirthDate,
                    DateOfHire = message.Payload.DateOfHire,
                    Registry = message.Payload.Registry,
                    MotherName = message.Payload.MotherName,
                    Company = message.Payload.Company,
                    Login = message.Payload.Login,
                    CorporateEmail = message.Payload.CorporateEmail,
                    PersonalEmail = message.Payload.PersonalEmail,
                    StatusUserProfile = message.Payload.StatusUserProfile,
                    TypeUserProfile = message.Payload.TypeUserProfile,
                    LoginManager = message.Payload.LoginManager,
                    PositionHeldId = message.Payload.PositionHeldId,
                    FiscalNumberIdentity = message.Payload.FiscalNumberIdentity,
                    ContractorCnpj = message.Payload.ContractorCnpj,
                    ContractorName = message.Payload.ContractorName,
                    CreatedBy = message.Payload.CreatedBy
                };


                var result = await _handler.Handle(command, context.CancellationToken).ConfigureAwait(false);

                if (result.IsSuccess)
                {
                    await context.Publish(new UserProfilePersisted
                    {
                        CorrelationId = message.CorrelationId,
                        DatabaseId = result.Data is Guid dbId ? dbId : Guid.Empty // Se você devolver o ID no Data
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
