namespace OneID.Domain.Entities.UserContext
{
#nullable disable
    public sealed class RecentAdmission
    {
        public string AccountId { get; init; }
        public string FullName { get; init; }
        public string JobTitleName { get; init; }
        public string DepartmentName { get; init; }
        public string Company { get; init; }
        public string LoginManager { get; init; }
        public DateTimeOffset CreatedAt { get; init; }

    }
}
