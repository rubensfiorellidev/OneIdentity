namespace OneID.Domain.Entities.UserContext
{
#nullable disable
    public class AccountPjAdmissionStaging
    {
        public AccountPjAdmissionStaging()
        {
            Status = "Pending";
        }

        public Guid CorrelationId { get; init; }
        public string FullName { get; init; }
        public string SocialName { get; init; }
        public string Cpf { get; init; }
        public string CpfHash { get; init; }
        public bool ForeignWorker { get; init; }
        public string FiscalNumberIdentity { get; init; }
        public string FiscalNumberIdentityHash { get; init; }
        public DateTime StartDate { get; init; }
        public DateTime? EndDate { get; init; }
        public string ContractorCnpj { get; init; }
        public string ContractorName { get; init; }
        public string PositionHeldId { get; init; }
        public string RegionalId { get; init; }
        public string RegionalName { get; init; }
        public string RegionalState { get; init; }
        public string ManagerId { get; init; }
        public string CostCenterId { get; init; }
        public string DepartmentId { get; init; }
        public string Login { get; init; }
        public string LoginHash { get; init; }
        public bool EmailAccess { get; init; }
        public string PersonalEmail { get; init; }
        public string PersonalEmailHash { get; init; }
        public string CorporateEmail { get; init; }
        public string CorporateEmailHash { get; init; }
        public bool VpnAccess { get; init; }
        public string Comments { get; init; }
        public string CreatedBy { get; init; }
        public DateTimeOffset CreatedAt { get; init; }
        public string Status { get; init; }
    }
}
