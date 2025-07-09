using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OneID.Domain.Entities.UserContext;

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
                   .HasMaxLength(14);

            builder.Property(u => u.CpfHash)
                   .HasMaxLength(128);

            builder.Property(u => u.BirthDate)
                   .HasColumnType("date");

            builder.Property(u => u.DateOfHire)
                   .HasColumnType("date");

            builder.Property(u => u.DateOfFired)
                   .HasColumnType("date");

            builder.Property(u => u.Registry)
                   .HasMaxLength(50);

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

            builder.Property(u => u.StatusUserAccount)
                   .HasMaxLength(50);

            builder.Property(u => u.TypeUserAccount)
                   .HasMaxLength(50);

            builder.Property(u => u.IsInactive);

            builder.Property(u => u.LoginManager)
                   .HasMaxLength(100);

            builder.Property(u => u.PositionHeldId)
                   .HasMaxLength(100);

            builder.Property(u => u.FiscalNumberIdentity)
                   .HasMaxLength(50);

            builder.Property(u => u.FiscalNumberIdentityHash)
                   .HasMaxLength(255);

            builder.Property(u => u.ContractorCnpj)
                   .HasMaxLength(18);

            builder.Property(u => u.ContractorCnpjHash)
                   .HasMaxLength(256);

            builder.Property(u => u.ContractorName)
                   .HasMaxLength(150);

            builder.Property(u => u.CreatedBy)
                   .HasMaxLength(100);

            builder.Property(u => u.ProvisioningAt)
                   .HasColumnType("timestamptz");

            builder.Property(u => u.UpdatedBy).HasMaxLength(100);
            builder.Property(u => u.UpdatedAt).HasColumnType("timestamptz");

            builder.HasIndex(u => u.Cpf).HasDatabaseName("idx_user_account_cpf");
            builder.HasIndex(u => u.Login).HasDatabaseName("idx_user_account_login");
            builder.HasIndex(u => u.CorporateEmail).HasDatabaseName("idx_user_account_corporate_email");


        }
    }


}
