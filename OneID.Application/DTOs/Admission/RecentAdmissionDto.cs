namespace OneID.Application.DTOs.Admission
{
    public record RecentAdmissionDto
    {
        public string AccountId { get; init; } = default!;
        public string FullName { get; init; } = default!;
        public string JobTitleName { get; init; } = default!;
        public string DepartmentName { get; init; } = default!;
        public string Company { get; init; } = default!;
        public string LoginManager { get; init; } = default!;
        public DateTimeOffset ProvisioningAt { get; init; }
    }

}
