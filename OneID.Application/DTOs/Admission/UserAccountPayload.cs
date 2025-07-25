﻿using OneID.Domain.ValueObjects;

#nullable disable
namespace OneID.Application.DTOs.Admission
{
    public record UserAccountPayload
    {
        public string FirstName { get; init; }
        public string LastName { get; init; }
        public string FullName { get; init; }
        public string SocialName { get; init; }
        public string Email { get; init; }
        public string Password { get; init; }
        public string Cpf { get; init; }
        public DateTime BirthDate { get; init; }
        public DateTime StartDate { get; init; }
        public string Registry { get; init; }
        public string MotherName { get; init; }
        public string Company { get; init; }
        public string Login { get; init; }
        public string CorporateEmail { get; init; }
        public string PersonalEmail { get; init; }
        public string PhoneNumber { get; init; }
        public UserAccountStatus StatusUserAccount { get; init; }
        public TypeUserAccount TypeUserAccount { get; init; }
        public string LoginManager { get; init; }
        public string JobTitleId { get; init; }
        public string FiscalNumberIdentity { get; init; }
        public string ContractorCnpj { get; init; }
        public string ContractorName { get; init; }
        public string CreatedBy { get; init; }

    }
}
