#nullable disable
namespace OneID.Domain.Entities.Packages
{
    public class AccessPackageCondition
    {
        public AccessPackageCondition(string accessPackageId, string department, string jobTitle)
        {
            AccessPackageId = accessPackageId;
            Department = department;
            JobTitle = jobTitle;
        }

        public string Id { get; private set; }
        public string AccessPackageId { get; private set; }
        public string Department { get; private set; }
        public string JobTitle { get; private set; }
        public string DepartmentId { get; private set; }
        public string JobTitleId { get; private set; }

        public AccessPackage AccessPackage { get; private set; }
    }

}
