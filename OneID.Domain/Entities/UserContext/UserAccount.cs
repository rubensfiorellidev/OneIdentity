using OneID.Domain.Abstractions.EventsContext;
using OneID.Domain.Abstractions.Validations;
using OneID.Domain.Contracts;
using OneID.Domain.Enums;
using OneID.Domain.Notifications;

#nullable disable

namespace OneID.Domain.Entities.UserContext
{
    public class UserAccount : IAggregateRoot
    {
        private readonly List<Event> _events = [];
        private readonly List<Notification> _notifications = [];

        public Guid CorrelationId { get; private set; }


        public string Id { get; private set; }
        public DateTimeOffset ProvisioningAt { get; private set; }
        public DateTimeOffset? UpdatedAt { get; private set; }
        public string CreatedBy { get; private set; }
        public string UpdatedBy { get; private set; }
        public string FullName { get; private set; }
        public string FirstName { get; private set; }
        public string LastName { get; private set; }
        public string SocialName { get; private set; }
        public string Cpf { get; private set; }
        public string CpfHash { get; private set; }
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
        public EnumStatusUserAccount StatusUserAccount { get; private set; }
        public EnumTypeUserAccount TypeUserAccount { get; private set; }
        public bool IsInactive { get; private set; }
        public string LoginManager { get; private set; }
        public string PositionHeldId { get; private set; }
        public string FiscalNumberIdentity { get; private set; }
        public string FiscalNumberIdentityHash { get; private set; }
        public string ContractorCnpj { get; private set; }
        public string ContractorCnpjHash { get; private set; }
        public string ContractorName { get; private set; }

        public IReadOnlyCollection<Event> Events => _events.AsReadOnly();
        public IReadOnlyCollection<Notification> Notifications => _notifications.AsReadOnly();


        public UserAccount(string id, string createdBy)
        {
            if (string.IsNullOrWhiteSpace(id)) throw new ArgumentNullException(nameof(id));

            Id = id;
            CreatedBy = createdBy;
            ProvisioningAt = DateTimeOffset.UtcNow;

            IsInactive = true;
        }

        public void Activate(string updatedBy)
        {
            if (!IsInactive)
                throw new InvalidOperationException("User is already active.");

            IsInactive = false;
            PrepareForUpdate(updatedBy);
        }


        public void AddEvent(Event domainEvent)
        {
            _events.Add(domainEvent);
        }

        public void ClearEvents()
        {
            _events.Clear();
        }

        public void AddNotification(Notification notification)
        {
            if (notification != null)
                _notifications.Add(notification);
        }

        public void AddNotifications(IEnumerable<Notification> notifications)
        {
            if (notifications != null)
                _notifications.AddRange(notifications);
        }

        public void PrepareForUpdate(string updatedBy)
        {
            UpdatedAt = DateTimeOffset.UtcNow;
            UpdatedBy = string.IsNullOrWhiteSpace(updatedBy) ? null : updatedBy;
        }

        public bool IsValid()
        {
            var contracts = CreateUserProfileValidator.Validate(this);
            AddNotifications(contracts.Notifications);
            return contracts.HasNotifications();
        }

        public override bool Equals(object obj)
        {
            return obj is UserAccount other && Id == other.Id;
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        public void SetFirstName(string firstName)
        {
            FirstName = firstName;
        }

        public void SetLastname(string lastName)
        {
            LastName = lastName;
        }

        public void SetCorrelationId(Guid correlationId)
        {
            CorrelationId = correlationId;
        }
        public void SetFullName(string fullName)
        {
            if (string.IsNullOrWhiteSpace(fullName))
                throw new ArgumentException("Full name cannot be empty", nameof(fullName));

            FullName = fullName;
        }

        public void SetSocialName(string socialName)
        {
            SocialName = socialName;
        }

        public void SetCpf(string cpf)
        {
            if (string.IsNullOrWhiteSpace(cpf))
                throw new ArgumentException("CPF cannot be empty", nameof(cpf));

            Cpf = cpf;
        }

        public void SetBirthDate(DateTime birthDate)
        {
            BirthDate = birthDate;
        }

        public void SetDateOfHire(DateTime dateOfHire)
        {
            DateOfHire = dateOfHire;
        }

        public void SetRegistry(string registry)
        {
            Registry = registry;
        }

        public void SetMotherName(string motherName)
        {
            MotherName = motherName;
        }

        public void SetCompany(string company)
        {
            Company = company;
        }

        public void SetLogin(string login)
        {
            Login = login;
        }

        public void SetCorporateEmail(string corporateEmail)
        {
            CorporateEmail = corporateEmail;
        }

        public void SetPersonalEmail(string personalEmail)
        {
            PersonalEmail = personalEmail;
        }

        public void SetStatusUserProfile(EnumStatusUserAccount status)
        {
            StatusUserAccount = status;
        }

        public void SetTypeUserProfile(EnumTypeUserAccount type)
        {
            TypeUserAccount = type;
        }

        public void SetLoginManager(string loginManager)
        {
            LoginManager = loginManager;
        }

        public void SetPositionHeldId(string positionHeldId)
        {
            PositionHeldId = positionHeldId;
        }

        public void SetFiscalNumberIdentity(string fiscalNumberIdentity)
        {
            FiscalNumberIdentity = fiscalNumberIdentity;
        }

        public void SetContractor(string contractorCnpj, string contractorName)
        {
            ContractorCnpj = contractorCnpj;
            ContractorName = contractorName;
        }

        public void ApplyHashes(string cpfHash,
                        string corporateEmailHash,
                        string loginHash,
                        string fiscalNumberIdentityHash,
                        string contractorCnpjHash)
        {
            if (string.IsNullOrWhiteSpace(cpfHash))
                throw new ArgumentException("CPF hash cannot be null or empty", nameof(cpfHash));

            CpfHash = cpfHash;

            if (!string.IsNullOrWhiteSpace(corporateEmailHash))
                CorporateEmailHash = corporateEmailHash;

            if (!string.IsNullOrWhiteSpace(loginHash))
                LoginHash = loginHash;

            if (!string.IsNullOrWhiteSpace(fiscalNumberIdentityHash))
                FiscalNumberIdentityHash = fiscalNumberIdentityHash;

            if (!string.IsNullOrWhiteSpace(contractorCnpjHash))
                ContractorCnpjHash = contractorCnpjHash;
        }

        public void ApplyCrytos(string cpf,
                        string corporateEmail,
                        string login,
                        string registry,
                        string fiscalNumberIdentity,
                        string contractorCnpj)
        {
            if (!string.IsNullOrWhiteSpace(cpf))
                Cpf = cpf;

            if (!string.IsNullOrWhiteSpace(corporateEmail))
                CorporateEmail = corporateEmail;

            if (!string.IsNullOrWhiteSpace(login))
                Login = login;

            if (!string.IsNullOrWhiteSpace(registry))
                Registry = registry;

            if (!string.IsNullOrWhiteSpace(fiscalNumberIdentity))
                FiscalNumberIdentity = fiscalNumberIdentity;

            if (!string.IsNullOrWhiteSpace(contractorCnpj))
                ContractorCnpj = contractorCnpj;
        }


    }
}
