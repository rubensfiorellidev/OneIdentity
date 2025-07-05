using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OneID.Domain.Entities.UserContext;

namespace OneID.Data.Mappings
{
    internal class UserProfileMap : IEntityTypeConfiguration<UserProfile>
    {
        public void Configure(EntityTypeBuilder<UserProfile> builder)
        {
            builder.ToTable("tb_oneid_user_profiles");

            builder.HasKey(u => u.Id);
            builder.Property(u => u.Id)
                   .HasColumnName("id")
                   .HasMaxLength(100)
                   .IsRequired();

            builder.Property(u => u.FullName)
                   .HasColumnName("full_name")
                   .HasMaxLength(300);

            builder.Property(u => u.FirstName)
                   .HasColumnName("first_name")
                   .HasMaxLength(100);

            builder.Property(u => u.LastName)
                   .HasColumnName("last_name")
                   .HasMaxLength(100);

            builder.Property(u => u.SocialName)
                   .HasColumnName("social_name")
                   .HasMaxLength(100);

            builder.Property(u => u.Cpf)
                   .HasColumnName("cpf")
                   .HasMaxLength(14);

            builder.Property(u => u.BirthDate)
                   .HasColumnName("birth_date")
                   .HasColumnType("timestamptz"); 

            builder.Property(u => u.DateOfHire)
                   .HasColumnName("date_of_hire")
                   .HasColumnType("timestamptz");

            builder.Property(u => u.DateOfFired)
                   .HasColumnName("date_of_fired")
                   .HasColumnType("timestamptz");

            builder.Property(u => u.Registry)
                   .HasColumnName("registry")
                   .HasMaxLength(50);

            builder.Property(u => u.MotherName)
                   .HasColumnName("mother_name")
                   .HasMaxLength(150);

            builder.Property(u => u.Company)
                   .HasColumnName("company")
                   .HasMaxLength(150);

            builder.Property(u => u.Login)
                   .HasColumnName("login")
                   .HasMaxLength(100);

            builder.Property(u => u.LoginHash)
                   .HasColumnName("login_hash")
                   .HasMaxLength(256);

            builder.Property(u => u.CorporateEmail)
                   .HasColumnName("corporate_email")
                   .HasMaxLength(150);

            builder.Property(u => u.CorporateEmailHash)
                   .HasColumnName("corporate_email_hash")
                   .HasMaxLength(256);

            builder.Property(u => u.PersonalEmail)
                   .HasColumnName("personal_email")
                   .HasMaxLength(150);

            builder.Property(u => u.PersonalEmailHash)
                   .HasColumnName("personal_email_hash")
                   .HasMaxLength(256);

            builder.Property(u => u.StatusUserProfile)
                   .HasColumnName("status_user_profile")
                   .HasConversion<int>();

            builder.Property(u => u.TypeUserProfile)
                   .HasColumnName("type_user_profile")
                   .HasConversion<int>();

            builder.Property(u => u.IsInactive)
                   .HasColumnName("is_inactive");

            builder.Property(u => u.LoginManager)
                   .HasColumnName("login_manager")
                   .HasMaxLength(100);

            builder.Property(u => u.PositionHeldId)
                   .HasColumnName("position_held_id")
                   .HasMaxLength(100);

            builder.Property(u => u.FiscalNumberIdentity)
                   .HasColumnName("fiscal_number_identity")
                   .HasMaxLength(50);

            builder.Property(u => u.ContractorCnpj)
                   .HasColumnName("contractor_cnpj")
                   .HasMaxLength(18);

            builder.Property(u => u.ContractorName)
                   .HasColumnName("contractor_name")
                   .HasMaxLength(150);

            builder.Property(u => u.CreatedBy)
                   .HasColumnName("created_by")
                   .HasMaxLength(100);

            builder.Property(u => u.ProvisioningAt)
                   .HasColumnName("provisioning_at")
                   .HasColumnType("timestamptz");

            builder.Property<string>("UpdatedBy")
                   .HasColumnName("updated_by")
                   .HasMaxLength(100);

            builder.Property<DateTimeOffset?>("UpdatedAt")
                   .HasColumnName("updated_at")
                   .HasColumnType("timestamptz");

            builder.HasIndex(u => u.Cpf)
                   .HasDatabaseName("idx_user_profile_cpf");

            builder.HasIndex(u => u.Login)
                   .HasDatabaseName("idx_user_profile_login");

            builder.HasIndex(u => u.CorporateEmail)
                   .HasDatabaseName("idx_user_profile_corporate_email");
        }
    }


}
