namespace OneID.Application.DTOs.Packages
{
    public readonly record struct AccessPackageConditionDto
    {
        public string Department { get; init; }
        public string JobTitle { get; init; }

        public AccessPackageConditionDto(string department, string jobTitle)
        {
            Department = department;
            JobTitle = jobTitle;
        }
    }

}
