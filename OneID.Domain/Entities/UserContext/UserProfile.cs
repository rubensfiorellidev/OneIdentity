using OneID.Domain.Abstractions.Events;
using OneID.Domain.Abstractions.Validations;
using OneID.Domain.Contracts.Validations;
using OneID.Domain.Enums;
using OneID.Domain.Notifications;

#nullable disable

namespace OneID.Domain.Entities.UserContext
{
    public class UserProfile : BaseEntity, IContract
    {
        private readonly List<Event> _events = [];

        public UserProfile(List<Notification> notifications) : base(notifications)
        {
        }

        public string FullName { get; private set; }
        public string FirstName { get; private set; }
        public string LastName { get; private set; }
        public string SocialName { get; private set; }
        public string Cpf { get; private set; }
        public DateTime BirthDate { get; private set; }
        public DateTime DateOfHire { get; private set; }
        public DateTime? DateOfFired { get; private set; }
        public string Registry { get; private set; }
        public string MotherName { get; private set; }
        public string Company { get; private set; }
        public string Login { get; private set; }
        public string LoginHash { get; private set; }
        public string CorporateEmail { get; private set; }
        public string CorporateEmailHash { get; private set; }
        public string PersonalEmail { get; private set; }
        public string PersonalEmailHash { get; private set; }
        public EnumStatusUserProfile StatusUserProfile { get; private set; }
        public EnumTypeUserProfile TypeUserProfile { get; set; }
        public bool IsInactive { get; private set; }
        public string LoginManager { get; private set; }
        public string PositionHeldId { get; private set; }
        public string FiscalNumberIdentity { get; private set; }
        public string ContractorCnpj { get; private set; }
        public string ContractorName { get; private set; }

        public override bool Validation()
        {
            var contracts = CreateUserProfileValidator.Validate(this);

            AddNotificationToStack([.. contracts.Notifications]);

            return contracts.HasNotifications();
        }
    }
}
