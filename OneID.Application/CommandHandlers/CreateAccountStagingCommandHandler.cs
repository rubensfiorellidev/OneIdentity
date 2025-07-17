using Microsoft.Extensions.Logging;
using OneID.Application.Commands;
using OneID.Application.Interfaces.Builders;
using OneID.Application.Interfaces.CQRS;
using OneID.Application.Interfaces.Repositories;
using OneID.Application.Interfaces.Services;
using OneID.Domain.Entities.UserContext;
using OneID.Domain.Interfaces;
using OneID.Domain.Results;

namespace OneID.Application.CommandHandlers
{
    public class CreateAccountStagingCommandHandler : ICommandHandler<CreateAccountStagingCommand, IResult>
    {
        private readonly IAddUserAccountStagingRepository _repository;
        private readonly ILogger<CreateAccountStagingCommandHandler> _logger;
        private readonly IUserAccountStagingBuilder _builder;
        private readonly IDeduplicationKeyRepository _keyRepository;
        private readonly IDeduplicationRepository _deduplicationRepository;
        private readonly ISender _sender;
        private readonly ILoggedUserAccessor _userAccessor;

        public CreateAccountStagingCommandHandler(
            IAddUserAccountStagingRepository repository,
            ILogger<CreateAccountStagingCommandHandler> logger,
            IUserAccountStagingBuilder userAccountStagingBuilder,
            IDeduplicationKeyRepository keyRepository,
            IDeduplicationRepository deduplicationRepository,
            ISender sender,
            ILoggedUserAccessor userAccessor)
        {
            _repository = repository;
            _logger = logger;
            _builder = userAccountStagingBuilder;
            _keyRepository = keyRepository;
            _deduplicationRepository = deduplicationRepository;
            _sender = sender;
            _userAccessor = userAccessor;
        }

        public async Task<IResult> Handle(CreateAccountStagingCommand command, CancellationToken cancellationToken)
        {
            var request = await _builder.BuildAsync(command.Request);

            if (await _keyRepository.ExistsAsync(request.CpfHash, "create-account", cancellationToken))
            {
                return Result.Conflict("Já existe um processo de admissão em andamento para esse CPF.");
            }

            if (await _deduplicationRepository.ExistsAsync(request.CorrelationId, cancellationToken))
            {
                return Result.Conflict("Esse CorrelationId já existe.");
            }

            var staging = new AccountAdmissionStaging
            {
                CorrelationId = request.CorrelationId,
                FirstName = request.FirstName,
                LastName = request.LastName,
                FullName = request.FullName,
                SocialName = request.SocialName,
                Cpf = request.Cpf,
                CpfHash = request.CpfHash,
                FiscalNumberIdentity = request.FiscalNumberIdentity,
                FiscalNumberIdentityHash = request.FiscalNumberIdentityHash,
                StartDate = request.StartDate,
                ContractorCnpj = request.ContractorCnpj,
                ContractorName = request.ContractorName,
                JobTitleId = request.JobTitleId,
                JobTitleName = request.JobTitleName,
                DepartmentId = request.DepartmentId,
                DepartmentName = request.DepartmentName,
                Login = request.Login,
                LoginHash = request.LoginHash,
                PersonalEmail = request.PersonalEmail,
                PersonalEmailHash = request.PersonalEmailHash,
                CorporateEmail = request.CorporateEmail,
                CorporateEmailHash = request.CorporateEmailHash,
                PhoneNumber = request.PhoneNumber,
                BirthDate = request.BirthDate,
                Company = request.Company,
                MotherName = request.MotherName,
                Registry = request.Registry,
                LoginManager = request.LoginManager,
                TypeUserAccount = request.TypeUserAccount,

                CreatedBy = command.CreatedBy,
                CreatedAt = DateTimeOffset.UtcNow,
                Status = "Pending"
            };

            await _repository.SaveAsync(staging, cancellationToken);
            await _keyRepository.SaveAsync(request.CpfHash, "create-account", cancellationToken);
            await _deduplicationRepository.SaveAsync(request.CorrelationId, "create-account", cancellationToken);

            //await _sender.SendAsync(new SendTotpNotificationCommand(
            //    request.CorrelationId,
            //    _userAccessor.GetEmail(),
            //    _userAccessor.GetName(),
            //    _userAccessor.GetPhone()

            //), cancellationToken);

            await _sender.SendAsync(new SendTotpNotificationCommand(
                request.CorrelationId,
                "rubensfiorelli@outlook.com",
                "Rubens Fiorelli",
                "+5511999999999"
            ), cancellationToken);



            _logger.LogInformation("Dados salvos em staging - CorrelationId: {CorrelationId}", request.CorrelationId);
            return Result.Success("Staging salvo com sucesso.", request);
        }
    }

}
