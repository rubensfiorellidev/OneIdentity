using OneID.Application.Interfaces.CQRS;
using OneID.Domain.ValueObjects;

#nullable disable
namespace OneID.Application.Commands
{
    public record CreateAccountDatabaseCommand : ICommand
    {
        public Guid CorrelationId { get; init; }


        public Guid KeycloakUserId { get; init; }
        public string FullName { get; init; }
        public string SocialName { get; init; }
        public string Cpf { get; init; }
        public DateTime BirthDate { get; init; }
        public DateTime DateOfHire { get; init; }
        public string Registry { get; init; }
        public string MotherName { get; init; }
        public string Company { get; init; }
        public string Login { get; init; }
        public string CorporateEmail { get; init; }
        public string PersonalEmail { get; init; }
        public string PhoneNumber { get; init; }
        public UserAccountStatus StatusUserAccount { get; init; }
        public TypeUserAccount TypeUserAccount { get; init; }
        public string LoginManager { get; init; }
        public string JobTitleId { get; init; }
        public string FiscalNumberIdentity { get; init; }
        public string ContractorCnpj { get; init; }
        public string ContractorName { get; init; }
        public string CreatedBy { get; init; }

    }
}
