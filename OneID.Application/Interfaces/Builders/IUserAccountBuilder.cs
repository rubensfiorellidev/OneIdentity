﻿using OneID.Domain.Entities.UserContext;
using OneID.Domain.ValueObjects;

namespace OneID.Application.Interfaces.Builders
{
    public interface IUserAccountBuilder
    {
        IUserAccountBuilder WithCorrelationId(Guid correlationId);
        IUserAccountBuilder WithFirstName(string firstName);
        IUserAccountBuilder WithLastName(string lastName);
        IUserAccountBuilder WithFullName(string fullName);
        IUserAccountBuilder WithSocialName(string socialName);
        IUserAccountBuilder WithCpf(string cpf);
        IUserAccountBuilder WithBirthDate(DateTime birthDate);
        IUserAccountBuilder WithDateOfHire(DateTime dateOfHire);
        IUserAccountBuilder WithRegistry(string registry);
        IUserAccountBuilder WithMotherName(string motherName);
        IUserAccountBuilder WithCompany(string company);
        IUserAccountBuilder WithLogin(string login);
        IUserAccountBuilder WithCorporateEmail(string corporateEmail);
        IUserAccountBuilder WithPersonalEmail(string personalEmail);
        IUserAccountBuilder WithPhoneNumber(string phoneNumber);
        IUserAccountBuilder WithStatusUserAccount(UserAccountStatus status);
        IUserAccountBuilder WithTypeUserAccount(TypeUserAccount type);
        IUserAccountBuilder WithLoginManager(string loginManager);
        IUserAccountBuilder WithJobTitle(string jobTitleName);
        IUserAccountBuilder WithJobTitleId(string jobTitleId);
        IUserAccountBuilder WithDepartment(string departmentName);
        IUserAccountBuilder WithDepartmentId(string departmentId);
        IUserAccountBuilder WithFiscalNumberIdentity(string fiscalNumberIdentity);
        IUserAccountBuilder WithContractor(string contractorCnpj, string contractorName);
        IUserAccountBuilder WithCreatedBy(string createdBy);
        IUserAccountBuilder WithKeycloakUserId(Guid keycloakUserId);
        UserAccount Build();
    }

}
