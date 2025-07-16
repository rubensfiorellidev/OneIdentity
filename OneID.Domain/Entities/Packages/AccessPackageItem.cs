#nullable disable
namespace OneID.Domain.Entities.Packages
{
    public class AccessPackageItem
    {
        protected AccessPackageItem() { }

        public AccessPackageItem(string packageId, string type, string value)
        {
            AccessPackageId = packageId;
            Type = type;
            Value = value;
        }

        public string Id { get; private set; }
        public string AccessPackageId { get; private set; }
        public string Type { get; private set; } // Ex: AD_GROUP, SAP_ROLE, SALESFORCE_PROFILE
        public string Value { get; private set; }

        public AccessPackage AccessPackage { get; private set; }
    }

}
