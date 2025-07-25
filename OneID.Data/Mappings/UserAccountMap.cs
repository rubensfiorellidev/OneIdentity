﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OneID.Domain.Entities.DepartmentContext;
using OneID.Domain.Entities.JobTitleContext;
using OneID.Domain.Entities.UserContext;
using OneID.Domain.ValueObjects;

namespace OneID.Data.Mappings
{
    internal class UserAccountMap : IEntityTypeConfiguration<UserAccount>
    {
        public void Configure(EntityTypeBuilder<UserAccount> builder)
        {
            builder.ToTable("tb_oneid_user_accounts");

            builder.HasKey(u => u.Id);

            builder.Property(u => u.Id)
                   .HasMaxLength(100)
                   .IsRequired();

            builder.Property(x => x.CorrelationId)
                   .IsRequired();

            builder.Property(u => u.FullName)
                   .HasMaxLength(300);

            builder.Property(u => u.FirstName)
                   .HasMaxLength(100);

            builder.Property(u => u.LastName)
                   .HasMaxLength(100);

            builder.Property(u => u.SocialName)
                   .HasMaxLength(100);

            builder.Property(u => u.Cpf)
                   .HasMaxLength(150);

            builder.Property(u => u.CpfHash)
                   .HasMaxLength(256);

            builder.Property(u => u.BirthDate)
                   .HasColumnType("date");

            builder.Property(u => u.StartDate)
                   .HasColumnType("date");

            builder.Property(u => u.DateOfFired)
                   .HasColumnType("date");

            builder.Property(u => u.Registry)
                   .HasMaxLength(150);

            builder.Property(u => u.MotherName)
                   .HasMaxLength(150);

            builder.Property(u => u.Company)
                   .HasMaxLength(150);

            builder.Property(u => u.Login)
                   .HasMaxLength(100);

            builder.Property(u => u.LoginHash)
                   .HasMaxLength(256);

            builder.Property(u => u.CorporateEmail)
                   .HasMaxLength(150);

            builder.Property(u => u.CorporateEmailHash)
                   .HasMaxLength(256);

            builder.Property(u => u.PersonalEmail)
                   .HasMaxLength(150);

            builder.Property(u => u.PersonalEmailHash)
                   .HasMaxLength(256);

            builder.Property(u => u.PhoneNumber)
                   .HasMaxLength(100);

            builder.Property(x => x.StatusUserAccount)
                   .HasConversion(
                        v => v.Value,
                        v => UserAccountStatus.From(v)
                   )
                   .HasColumnName("StatusUserAccount");

            builder.Property(x => x.TypeUserAccount)
                   .HasConversion(
                        v => v.Value,
                        v => TypeUserAccount.From(v)
                   )
                   .HasColumnName("TypeUserAccount");


            builder.Property(u => u.IsInactive);

            builder.Property(u => u.LoginManager)
                   .HasMaxLength(100);

            builder.Property(u => u.JobTitleId)
                    .IsRequired(false)
                    .HasMaxLength(100);

            builder.Property(u => u.JobTitleName)
                   .HasMaxLength(150);

            builder.Property(u => u.DepartmentId)
                   .IsRequired(false)
                   .HasMaxLength(100);

            builder.Property(u => u.DepartmentName)
                   .HasMaxLength(100);

            builder.Property(u => u.FiscalNumberIdentity)
                   .HasMaxLength(255);

            builder.Property(u => u.FiscalNumberIdentityHash)
                   .HasMaxLength(255);

            builder.Property(u => u.ContractorCnpj)
                   .HasMaxLength(255);

            builder.Property(u => u.ContractorCnpjHash)
                   .HasMaxLength(256);

            builder.Property(u => u.ContractorName)
                   .HasMaxLength(150);

            builder.Property(u => u.CreatedBy)
                   .HasMaxLength(100);

            builder.Property(u => u.ProvisioningAt)
                   .HasColumnType("timestamptz");

            builder.Property(u => u.UpdatedBy)
                   .HasMaxLength(100);

            builder.Property(u => u.UpdatedAt)
                   .HasColumnType("timestamptz");

            builder.Property(u => u.LastLoginAt)
                   .HasColumnType("timestamptz");

            builder.Property(u => u.KeycloakUserId);

            builder.Ignore(u => u.Events);
            builder.Ignore(u => u.Notifications);

            builder.HasOne<JobTitle>()
                   .WithMany()
                   .HasForeignKey(x => x.JobTitleId)
                   .OnDelete(DeleteBehavior.SetNull);

            builder.HasOne<Department>()
                   .WithMany()
                   .HasForeignKey(x => x.DepartmentId)
                   .OnDelete(DeleteBehavior.SetNull);

            // Indexes
            builder.HasIndex(u => u.Cpf).HasDatabaseName("idx_user_account_cpf");
            builder.HasIndex(u => u.Login).HasDatabaseName("idx_user_account_login");
            builder.HasIndex(u => u.CorporateEmail).HasDatabaseName("idx_user_account_corporate_email");

        }
    }


}
