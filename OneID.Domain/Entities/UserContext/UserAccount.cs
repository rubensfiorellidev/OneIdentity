﻿using OneID.Domain.Abstractions.EventsContext;
using OneID.Domain.Abstractions.Validations;
using OneID.Domain.Contracts;
using OneID.Domain.Contracts.Validations;
using OneID.Domain.Notifications;
using OneID.Domain.ValueObjects;

#nullable disable
namespace OneID.Domain.Entities.UserContext
{
    public class UserAccount : IAggregateRoot, IAuditableEntity, IContract, IValidation
    {
        private readonly List<Event> _events = [];
        private readonly List<Notification> _notifications = [];

        public string Id { get; private set; }
        public Guid CorrelationId { get; private set; }

        public DateTimeOffset ProvisioningAt { get; private set; }
        public DateTimeOffset? UpdatedAt { get; private set; }
        public string CreatedBy { get; private set; }
        public string UpdatedBy { get; private set; }

        public string FullName { get; private set; }
        public string FirstName { get; private set; }
        public string LastName { get; private set; }
        public string SocialName { get; private set; }
        public string MotherName { get; private set; }
        public DateTime BirthDate { get; private set; }

        public string Cpf { get; private set; }
        public string CpfHash { get; private set; }
        public string FiscalNumberIdentity { get; private set; }
        public string FiscalNumberIdentityHash { get; private set; }

        public string PersonalEmail { get; private set; }
        public string PersonalEmailHash { get; private set; }
        public string CorporateEmail { get; private set; }
        public string CorporateEmailHash { get; private set; }
        public string PhoneNumber { get; private set; }

        public string Registry { get; private set; }
        public string Company { get; private set; }
        public string JobTitleId { get; private set; }
        public string JobTitleName { get; private set; }
        public string DepartmentId { get; private set; }
        public string DepartmentName { get; private set; }
        public string ContractorCnpj { get; private set; }
        public string ContractorCnpjHash { get; private set; }
        public string ContractorName { get; private set; }
        public DateTime StartDate { get; private set; }
        public DateTime? DateOfFired { get; private set; }

        public UserAccountStatus StatusUserAccount { get; private set; }
        public TypeUserAccount TypeUserAccount { get; private set; }
        public bool IsInactive { get; private set; }
        public bool IsActive => !IsInactive;

        public string Login { get; private set; }
        public string LoginHash { get; private set; }
        public string LoginManager { get; private set; }
        public DateTimeOffset? LastLoginAt { get; private set; }

        public Guid KeycloakUserId { get; private set; }

        public IReadOnlyCollection<Event> Events => _events.AsReadOnly();
        public IReadOnlyCollection<Notification> Notifications => _notifications.AsReadOnly();

        private UserAccount() { }

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

        public void Deactivate(string updatedBy)
        {
            if (IsInactive)
                throw new InvalidOperationException("User is already inactive.");

            IsInactive = true;
            PrepareForUpdate(updatedBy);
        }

        public void PrepareForUpdate(string updatedBy)
        {
            UpdatedAt = DateTimeOffset.UtcNow;
            UpdatedBy = string.IsNullOrWhiteSpace(updatedBy) ? null : updatedBy;
        }

        public void SetLastLoginAt(DateTimeOffset lastLogin) => LastLoginAt = lastLogin;

        public void SetKeycloakUserId(Guid keycloakUserId)
        {
            if (KeycloakUserId == Guid.Empty)
                KeycloakUserId = keycloakUserId;
        }


        public void SetCorrelationId(Guid correlationId) => CorrelationId = correlationId;

        // Métodos de domínio (setters protegidos)
        public void SetFirstName(string firstName) => FirstName = firstName;
        public void SetLastname(string lastName) => LastName = lastName;
        public void SetFullName(string fullName)
        {
            if (string.IsNullOrWhiteSpace(fullName))
                throw new ArgumentException("Full name cannot be empty", nameof(fullName));

            FullName = fullName;
        }
        public void SetSocialName(string socialName) => SocialName = socialName;
        public void SetCpf(string cpf)
        {
            if (string.IsNullOrWhiteSpace(cpf))
                throw new ArgumentException("CPF cannot be empty", nameof(cpf));

            Cpf = cpf;
        }
        public void SetBirthDate(DateTime birthDate) => BirthDate = birthDate;
        public void SetDateOfHire(DateTime dateOfHire) => StartDate = dateOfHire;
        public void SetDateOfFired(DateTime? dateOfFired) => DateOfFired = dateOfFired;
        public void SetRegistry(string registry) => Registry = registry;
        public void SetMotherName(string motherName) => MotherName = motherName;
        public void SetCompany(string company) => Company = company;
        public void SetLogin(string login) => Login = login;
        public void SetCorporateEmail(string corporateEmail) => CorporateEmail = corporateEmail;
        public void SetPersonalEmail(string personalEmail) => PersonalEmail = personalEmail;
        public void SetPhoneNumber(string phoneNumber) => PhoneNumber = phoneNumber;
        public void SetLoginManager(string loginManager) => LoginManager = loginManager;
        public void SetJobTitleId(string jobTitleId) => JobTitleId = jobTitleId;
        public void SetJobTitle(string jobTitle) => JobTitleName = jobTitle;
        public void SetDepartmentId(string departimentId) => DepartmentId = departimentId;
        public void SetDepartment(string department) => DepartmentName = department;
        public void SetFiscalNumberIdentity(string fiscalNumberIdentity) => FiscalNumberIdentity = fiscalNumberIdentity;
        public void SetContractor(string contractorCnpj, string contractorName)
        {
            ContractorCnpj = contractorCnpj;
            ContractorName = contractorName;
        }

        public void SetStatusUserAccount(UserAccountStatus status)
        {
            ArgumentNullException.ThrowIfNull(status);

            StatusUserAccount = status;
        }

        public void SetTypeUserAccount(TypeUserAccount type)
        {
            ArgumentNullException.ThrowIfNull(type);

            TypeUserAccount = type;
        }

        public void ApplyHashes(string cpfHash,
                                string corporateEmailHash,
                                string personalEmailHash,
                                string loginHash,
                                string fiscalNumberIdentityHash,
                                string contractorCnpjHash)
        {
            if (string.IsNullOrWhiteSpace(cpfHash))
                throw new ArgumentException("CPF hash cannot be null or empty", nameof(cpfHash));

            CpfHash = cpfHash;
            CorporateEmailHash = corporateEmailHash;
            PersonalEmailHash = personalEmailHash;
            LoginHash = loginHash;
            FiscalNumberIdentityHash = fiscalNumberIdentityHash;
            ContractorCnpjHash = contractorCnpjHash;
        }

        public void ApplyCrytos(string cpf,
                                string corporateEmail,
                                string personalEmail,
                                string login,
                                string registry,
                                string fiscalNumberIdentity,
                                string contractorCnpj,
                                string loginManager)
        {
            Cpf = cpf;
            CorporateEmail = corporateEmail;
            PersonalEmail = personalEmail;
            Login = login;
            Registry = registry;
            FiscalNumberIdentity = fiscalNumberIdentity;
            ContractorCnpj = contractorCnpj;
            LoginManager = loginManager;
        }

        // Validação e eventos
        public bool IsValid()
        {
            var contracts = CreateUserProfileValidator.Validate(this);
            AddNotifications(contracts.Notifications);
            return contracts.HasNotifications();
        }

        public void AddEvent(Event domainEvent) => _events.Add(domainEvent);
        public void ClearEvents() => _events.Clear();
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

        public override bool Equals(object obj) => obj is UserAccount other && Id == other.Id;
        public override int GetHashCode() => Id.GetHashCode();
    }
}
