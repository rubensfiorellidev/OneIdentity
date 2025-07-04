using OneID.Domain.Notifications;

namespace OneID.Domain.Abstractions.Validations
{
    public partial class ContractValidations<T>
    {
        public ContractValidations<T> BirthDateIsOK(DateTime birthDate, string message, string propertyName)
        {
            if (birthDate.Year > DateTime.Today.Year - 14 || birthDate.Year < DateTime.Today.Year - 100)
                AddNotification(new Notification(message, propertyName));

            return this;
        }

    }

}
