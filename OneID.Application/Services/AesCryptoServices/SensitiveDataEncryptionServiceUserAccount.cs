using OneID.Application.Interfaces.AesCryptoService;
using OneID.Application.Interfaces.SensitiveData;
using OneID.Domain.Entities.UserContext;

namespace OneID.Application.Services.AesCryptoServices
{
    public class SensitiveDataEncryptionServiceUserAccount : ISensitiveDataEncryptionServiceUserAccount
    {
        private readonly ICryptoService _cryptoService;
        public SensitiveDataEncryptionServiceUserAccount(ICryptoService cryptoService)
        {
            _cryptoService = cryptoService ?? throw new ArgumentNullException(nameof(cryptoService));
        }

        public Task<UserAccount> EncryptSensitiveDataAsync(UserAccount account)
        {
            ArgumentNullException.ThrowIfNull(account);

            var encryptedCpf = _cryptoService.Encrypt(account.Cpf);
            var encryptedCorporateEmail = _cryptoService.Encrypt(account.CorporateEmail);
            var encryptedLogin = _cryptoService.Encrypt(account.Login);
            var encryptedRegistry = _cryptoService.Encrypt(account.Registry);
            var encryptedFiscalNumber = !string.IsNullOrEmpty(account.FiscalNumberIdentity)
                ? _cryptoService.Encrypt(account.FiscalNumberIdentity)
                : account.FiscalNumberIdentity;
            var encryptedContractorCnpj = !string.IsNullOrEmpty(account.ContractorCnpj)
                ? _cryptoService.Encrypt(account.ContractorCnpj)
                : account.ContractorCnpj;

            account.ApplyCrytos(
                encryptedCpf,
                encryptedCorporateEmail,
                encryptedLogin,
                encryptedRegistry,
                encryptedFiscalNumber,
                encryptedContractorCnpj
            );

            return Task.FromResult(account);
        }
    }

}
