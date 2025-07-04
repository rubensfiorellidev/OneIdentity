using OneID.Domain.Notifications;

namespace OneID.Domain.Abstractions.Validations
{
    public partial class ContractValidations<T>
    {
        public ContractValidations<T> CompanyIsOk(string company, short maxLength, short minLength, string message, string propertyName)
        {
            if (string.IsNullOrEmpty(company) || company.Length > maxLength || company.Length < minLength)
                AddNotification(new Notification(message, propertyName));

            return this;
        }

    }

}
