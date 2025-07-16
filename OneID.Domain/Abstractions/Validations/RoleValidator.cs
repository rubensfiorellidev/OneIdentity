using OneID.Domain.Entities.UserContext;

namespace OneID.Domain.Abstractions.Validations
{
    public class RoleValidator
    {
        public static ContractValidations<Role> Validate(Role role)
        {

            var validationResult = new ContractValidations<Role>()
                .NameIsOK(role.Name, 50, 5,
                      "Nome da role deve conter pelo menos 5 caracteres.", nameof(role.Name));

            return validationResult;

        }
    }
}
