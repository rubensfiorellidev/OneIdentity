using Microsoft.Extensions.Logging;
using OneID.Application.Interfaces;
using OneID.Domain.Helpers;

namespace OneID.Application.Abstractions
{
    public class AccountProvisioningOrchestrator : IAccountProvisioningOrchestrator
    {
        private readonly IUserLoginGenerator _loginGenerator;
        private readonly IKeycloakUserCreator _keycloakUserCreator;
        private readonly ILogger<AccountProvisioningOrchestrator> _logger;

        public AccountProvisioningOrchestrator(
            IUserLoginGenerator loginGenerator,
            IKeycloakUserCreator keycloakUserCreator,
            ILogger<AccountProvisioningOrchestrator> logger)
        {
            _loginGenerator = loginGenerator;
            _keycloakUserCreator = keycloakUserCreator;
            _logger = logger;
        }

        public async Task<string> ProvisionLoginAsync(string firstName, string lastName, CancellationToken cancellationToken)
        {
            var fullName = $"{firstName} {lastName}";
            var login = await _loginGenerator.GenerateLoginAsync(fullName, cancellationToken);
            var password = PasswordTempGenerator.GenerateTemporaryPassword();
            var email = $"{login}@company.com";

            _logger.LogInformation("Provisionando usuário {Login} no Keycloak...", login);

            await _keycloakUserCreator.CreateUserAsync(login, password, email, firstName, lastName, cancellationToken);

            _logger.LogInformation("Usuário {Login} criado no Keycloak com sucesso.", login);

            return login;
        }
    }

}
