using OneID.Domain.Notifications;

namespace OneID.Domain.Abstractions.Validations
{
    public partial class ContractValidations<T>
    {
        public ContractValidations<T> DateOfHireIsOK(DateTime dateOfHire, string message, string propertyName)
        {

            if (dateOfHire.Year < DateTime.Today.Year - 60)
                AddNotification(new Notification(message, propertyName));

            return this;
        }

    }

}
