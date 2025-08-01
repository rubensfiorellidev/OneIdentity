namespace OneID.WebApp.ViewModels
{
    public sealed record AllUserViewModel
    {
        public string Id { get; init; } = string.Empty;
        public string FirstName { get; init; } = string.Empty;
        public string LastName { get; init; } = string.Empty;
        public string Company { get; init; } = string.Empty;
        public string StatusUserAccount { get; init; } = string.Empty;
        public string TypeUserAccount { get; init; } = string.Empty;
        public string JobTitleName { get; init; } = string.Empty;
        public string CreatedBy { get; init; } = string.Empty;

        public string FullName => $"{FirstName} {LastName}";
        public bool IsSelected { get; set; } = false;
    }

}
