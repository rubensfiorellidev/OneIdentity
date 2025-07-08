using OneID.Domain.Enums;

namespace OneID.Domain.Entities.UserContext
{
#nullable disable
    public class AccountAdmissionStaging
    {
        public AccountAdmissionStaging()
        {
            Status = "Pending";
            CreatedAt = DateTimeOffset.UtcNow;
        }

        public Guid CorrelationId { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FullName { get; set; }
        public string SocialName { get; set; }
        public string Cpf { get; set; }
        public string CpfHash { get; set; }
        public string FiscalNumberIdentity { get; set; }
        public string FiscalNumberIdentityHash { get; set; }
        public DateTime BirthDate { get; set; }
        public DateTime StartDate { get; set; }
        public string Registry { get; set; }
        public string MotherName { get; set; }
        public string Company { get; set; }
        public string ContractorCnpj { get; set; }
        public string ContractorCnpjHash { get; set; }
        public string ContractorName { get; set; }
        public string PositionHeldId { get; set; }
        public string Login { get; set; }
        public string LoginHash { get; set; }
        public string LoginManager { get; set; }
        public string PersonalEmail { get; set; }
        public string PersonalEmailHash { get; set; }
        public string CorporateEmail { get; set; }
        public string CorporateEmailHash { get; set; }
        public string Comments { get; set; }
        public string CreatedBy { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public string Status { get; set; }

        public EnumStatusUserAccount StatusUserAccount { get; set; } = EnumStatusUserAccount.Inactive;
        public EnumTypeUserAccount TypeUserAccount { get; set; }
    }
}
