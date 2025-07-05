using OneID.Domain.Entities.UserContext;

namespace OneID.Domain.Abstractions.Validations
{
    public class CreateUserProfileValidator
    {
        public static ContractValidations<UserProfile> Validate(UserProfile account)
        {

            var validationResult = new ContractValidations<UserProfile>()
                .NameIsOK(account.FullName, 50, 10,
                      "O Nome completo deve ter entre 10 e 50 caracteres.", nameof(account.FullName))
                .CpfIsOK(account.Cpf, false, "Número de CPF inválido!", nameof(account.Cpf))
                .BirthDateIsOK(account.BirthDate, "Verifique a data de nascimento...", nameof(account.BirthDate))
                .DateOfHireIsOK(account.DateOfHire,
                      "A data de contratação não pode ser menor que a data de hoje!", nameof(account.DateOfHire))
                .NameIsOK(account.MotherName, 50, 10,
                      "O Nome completo da mãe deve ter entre 10 e 50 caracteres.", nameof(account.MotherName))
                .CompanyIsOk(account.Company, 50, 4,
                         "Informe a razão social ou nome fantasia da empresa!", nameof(account.Company));

            return validationResult;

        }
    }
}
