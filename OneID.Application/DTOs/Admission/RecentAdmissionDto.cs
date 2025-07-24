namespace OneID.Application.DTOs.Admission
{
    public readonly record struct RecentAdmissionDto
    {
        public string AccountId { get; init; }
        public string FullName { get; init; }
        public string JobTitleName { get; init; }
        public string DepartmentName { get; init; }
        public string Company { get; init; }
        public DateTimeOffset ProvisioningAt { get; init; }
    }

}
