using OneID.Domain.Notifications;

namespace OneID.Domain.Abstractions.Validations
{
    public partial class ContractValidations<T>
    {
        public ContractValidations<T> NameIsOK(string fullName, int maxLength, int minLength, string message, string propertyName)
        {
            if (string.IsNullOrEmpty(fullName) || fullName.Length > maxLength || fullName.Length < minLength)
                AddNotification(new Notification(message, propertyName));

            return this;
        }

    }

}
