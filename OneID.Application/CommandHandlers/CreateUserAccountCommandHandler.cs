using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OneID.Application.Commands;
using OneID.Application.Interfaces.Builders;
using OneID.Application.Interfaces.CQRS;
using OneID.Application.Interfaces.Repositories;
using OneID.Application.Interfaces.SensitiveData;
using OneID.Application.Interfaces.Services;
using OneID.Domain.Abstractions.DomainErrorCodes;
using OneID.Domain.Entities.UserContext;
using OneID.Domain.EventsFromDomain;
using OneID.Domain.Interfaces;
using OneID.Domain.Results;

namespace OneID.Application.CommandHandlers
{
    public sealed class CreateUserAccountCommandHandler : ICommandHandler<CreateUserAccountCommand, IResult>
    {
        private readonly IUserAccountBuilder _builder;
        private readonly IAddUserAccountRepository _repository;
        private readonly ILogger<CreateUserAccountCommandHandler> _logger;
        private readonly IHashService _hashService;
        private readonly ISensitiveDataEncryptionServiceUserAccount _sensitiveDataEncryptionServiceUserAccount;
        private readonly IEventDispatcher _dispatcher;

        public CreateUserAccountCommandHandler(
            IUserAccountBuilder builder,
            IAddUserAccountRepository repository,
            ILogger<CreateUserAccountCommandHandler> logger,
            IHashService hashService,
            ISensitiveDataEncryptionServiceUserAccount sensitiveDataEncryptionServiceUserAccount,
            IEventDispatcher dispatcher)
        {
            _builder = builder ?? throw new ArgumentNullException(nameof(builder));
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _hashService = hashService;
            _sensitiveDataEncryptionServiceUserAccount = sensitiveDataEncryptionServiceUserAccount;
            _dispatcher = dispatcher;
        }
        public async Task<IResult> Handle(CreateUserAccountCommand command, CancellationToken cancellationToken)
        {
            try
            {
                var userAccount = _builder
                    .WithFullName(command.FullName)
                    .WithSocialName(command.SocialName)
                    .WithCpf(command.Cpf)
                    .WithBirthDate(command.BirthDate)
                    .WithDateOfHire(command.DateOfHire)
                    .WithRegistry(command.Registry)
                    .WithMotherName(command.MotherName)
                    .WithCompany(command.Company)
                    .WithLogin(command.Login)
                    .WithCorporateEmail(command.CorporateEmail)
                    .WithPersonalEmail(command.PersonalEmail)
                    .WithStatusUserProfile(command.StatusUserProfile)
                    .WithTypeUserProfile(command.TypeUserProfile)
                    .WithLoginManager(command.LoginManager)
                    .WithPositionHeldId(command.PositionHeldId)
                    .WithFiscalNumberIdentity(command.FiscalNumberIdentity)
                    .WithContractor(command.ContractorCnpj, command.ContractorName)
                    .WithCreatedBy(command.CreatedBy)
                    .Build();

                if (userAccount.IsValid())
                {
                    try
                    {
                        userAccount = await _sensitiveDataEncryptionServiceUserAccount.EncryptSensitiveDataAsync(userAccount).ConfigureAwait(false);

                        var cpfHash = await _hashService.ComputeSha3HashAsync(userAccount.Cpf).ConfigureAwait(false);
                        var emailHash = await _hashService.ComputeSha3HashAsync(userAccount.CorporateEmail).ConfigureAwait(false);
                        var loginHash = await _hashService.ComputeSha3HashAsync(userAccount.Login).ConfigureAwait(false);
                        var fiscalHash = await _hashService.ComputeSha3HashAsync(userAccount.FiscalNumberIdentity).ConfigureAwait(false);
                        var contractorCnpjHash = await _hashService.ComputeSha3HashAsync(userAccount.ContractorCnpj).ConfigureAwait(false);

                        userAccount.ApplyHashes(cpfHash, emailHash, loginHash, fiscalHash, contractorCnpjHash);

                        var result = await _repository.AddAsync(userAccount, cancellationToken).ConfigureAwait(false);

                        if (result.IsSuccess)
                        {
                            var createdEvent = new UserAccountCreatedEvent(
                                 DateTimeOffset.UtcNow,
                                 userAccount.Id,
                                 nameof(UserAccount),
                                 userAccount.CreatedBy
                            );

                            userAccount.AddEvent(createdEvent);

                            await _dispatcher.HandleAsync(userAccount.Events, cancellationToken);
                            userAccount.ClearEvents();

                            _logger.LogInformation("UserAccount {Login} criado com sucesso e evento registrado.", userAccount.Login);

                            return Result.Success($"{userAccount.FullName} successfully created");
                        }

                        var failedEvent = new UserAccountCreationFailedEvent(
                            DateTimeOffset.UtcNow,
                            userAccount.Id,
                            nameof(UserAccount),
                            userAccount.CreatedBy
                        );

                        userAccount.AddEvent(failedEvent);

                        await _dispatcher.HandleAsync(userAccount.Events, cancellationToken);
                        userAccount.ClearEvents();

                        _logger.LogWarning("Falha ao persistir UserAccount {Login}. Evento de falha registrado.", userAccount.Login);

                        return result;

                    }
                    catch (ArgumentException error)
                    {
                        var errorCode = ErrorCodes.DbException;
                        _logger.LogError(error, "{ErrorCode}: {ErrorMessage}", errorCode, ErrorCodes.GetErrorMessage(errorCode));
                        return Result.Failure(ErrorCodes.GetErrorMessage(errorCode));
                    }
                }
                else
                {
                    _logger.LogWarning("Validação falhou para o usuário {Login}. Notificações: {Notifications}",
                        userAccount.Login, string.Join(", ", userAccount.Notifications.Select(n => n.Message)));

                    return Result.Failure(
                        "Dados inválidos.",
                        [.. userAccount.Notifications.Select(n => n.Message)]
                    );

                }
            }
            catch (ArgumentException error)
            {
                var errorCode = ErrorCodes.DbException;
                _logger.LogError(error, "{ErrorCode}: {ErrorMessage}", errorCode, ErrorCodes.GetErrorMessage(errorCode));
                return Result.Failure(ErrorCodes.GetErrorMessage(errorCode));
            }
            catch (DbUpdateConcurrencyException error)
            {
                var errorCode = ErrorCodes.DbConcurrencyError;
                _logger.LogError(error, "{ErrorCode}: {ErrorMessage} while creating account.", errorCode, ErrorCodes.GetErrorMessage(errorCode));
                return Result.Failure(ErrorCodes.GetErrorMessage(errorCode));
            }
            catch (Exception error)
            {
                var errorCode = ErrorCodes.ServiceUnexpectedError;
                _logger.LogError(error, "{ErrorCode}: {ErrorMessage} during request", errorCode, ErrorCodes.GetErrorMessage(errorCode));

                return Result.Failure(ErrorCodes.GetErrorMessage(errorCode));
            }
        }

    }
}
