using OneID.Application.Interfaces.AesCryptoService;
using OneID.Application.Interfaces.SensitiveData;
using OneID.Domain.Entities.UserContext;

#nullable disable
namespace OneID.Application.Services.AesCryptoServices
{
    public sealed class SensitiveDataDecryptionServiceUserAccount : ISensitiveDataDecryptionServiceUserAccount
    {
        private readonly ICryptoService _cryptoService;

        public SensitiveDataDecryptionServiceUserAccount(ICryptoService cryptoService)
        {
            _cryptoService = cryptoService ?? throw new ArgumentNullException(nameof(cryptoService));
        }

        public Task<UserAccount> DecryptSensitiveDataAsync(UserAccount account)
        {
            ArgumentNullException.ThrowIfNull(account);

            var decryptedCpf = !string.IsNullOrWhiteSpace(account.Cpf)
                ? _cryptoService.Decrypt(account.Cpf)
                : account.Cpf;

            var decryptedCorporateEmail = !string.IsNullOrWhiteSpace(account.CorporateEmail)
                ? _cryptoService.Decrypt(account.CorporateEmail)
                : account.CorporateEmail;

            var decryptedPersonalEmail = !string.IsNullOrWhiteSpace(account.PersonalEmail)
                ? _cryptoService.Decrypt(account.PersonalEmail)
                : account.PersonalEmail;

            var decryptedLogin = !string.IsNullOrWhiteSpace(account.Login)
                ? _cryptoService.Decrypt(account.Login)
                : account.Login;

            var decryptedRegistry = !string.IsNullOrWhiteSpace(account.Registry)
                ? _cryptoService.Decrypt(account.Registry)
                : account.Registry;

            var decryptedFiscalNumber = !string.IsNullOrWhiteSpace(account.FiscalNumberIdentity)
                ? _cryptoService.Decrypt(account.FiscalNumberIdentity)
                : account.FiscalNumberIdentity;

            var decryptedContractorCnpj = !string.IsNullOrWhiteSpace(account.ContractorCnpj)
                ? _cryptoService.Decrypt(account.ContractorCnpj)
                : account.ContractorCnpj;

            var decryptedLoginManager = !string.IsNullOrWhiteSpace(account.LoginManager)
                ? _cryptoService.Decrypt(account.LoginManager)
                : account.LoginManager;

            account.ApplyCrytos(
                decryptedCpf,
                decryptedCorporateEmail,
                decryptedPersonalEmail,
                decryptedLogin,
                decryptedRegistry,
                decryptedFiscalNumber,
                decryptedContractorCnpj,
                decryptedLoginManager

            );

            return Task.FromResult(account);
        }
    }

}
