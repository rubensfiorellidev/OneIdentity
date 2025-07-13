using OneID.Domain.ValueObjects;

namespace OneID.Application.DTOs.Admission
{
#nullable disable
    public record AccountRequest
    {
        public Guid CorrelationId { get; init; }

        public string FirstName { get; init; }
        public string LastName { get; init; }
        public string FullName { get; init; }
        public string SocialName { get; init; }
        public string Cpf { get; init; }
        public string CpfHash { get; init; }
        public DateTime BirthDate { get; init; }
        public string Registry { get; init; }
        public string MotherName { get; init; }
        public string Company { get; init; }
        public TypeUserAccount TypeUserAccount { get; init; }
        public string LoginManager { get; init; }
        public string FiscalNumberIdentity { get; init; }
        public string FiscalNumberIdentityHash { get; init; }
        public DateTime StartDate { get; init; }
        public DateTime? EndDate { get; init; }
        public string ContractorCnpj { get; init; }
        public string ContractorCnpjHash { get; init; }
        public string ContractorName { get; init; }
        public string JobTitleId { get; init; }
        public string DepartmentId { get; init; }
        public string DepartmentName { get; init; }
        public string JobTitleName { get; init; }
        public string Login { get; init; }
        public string LoginHash { get; init; }
        public string PersonalEmail { get; init; }
        public string PersonalEmailHash { get; init; }
        public string CorporateEmail { get; init; }
        public string CorporateEmailHash { get; init; }
        public string PhoneNumber { get; init; }
        public string Comments { get; init; }
        public string TotpCode { get; init; }

    }
}
