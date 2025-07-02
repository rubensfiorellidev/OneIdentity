using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OneID.Domain.Entities;

namespace OneID.Data.Mappings
{
    internal class ApplicationUserMap : IEntityTypeConfiguration<ApplicationUser>
    {
        public void Configure(EntityTypeBuilder<ApplicationUser> builder)
        {
            builder.ToTable("tb_oneid_users");

            builder.Property(u => u.Id)
                .HasColumnName("id")
                .HasColumnType("varchar")
                .HasMaxLength(26)
                .IsRequired();

            builder.Property(u => u.Fullname)
                .HasColumnName("fullname")
                .HasColumnType("varchar")
                .HasMaxLength(250)
                .IsRequired();

            builder.Property(u => u.Login)
                .HasColumnName("login")
                .HasColumnType("varchar")
                .HasMaxLength(150)
                .IsRequired();

            builder.Property(u => u.ProvisioningAt)
                .HasColumnName("provisioning_at")
                .HasColumnType("timestamptz")
                .HasDefaultValueSql("CURRENT_TIMESTAMP AT TIME ZONE 'UTC'")
                .IsRequired();

            builder.Property(u => u.UserName)
                .HasColumnName("user_name")
                .HasMaxLength(256);

            builder.Property(u => u.NormalizedUserName)
                .HasColumnName("normalized_user_name")
                .HasMaxLength(256);

            builder.Property(u => u.Email)
                .HasColumnName("email")
                .HasMaxLength(256);

            builder.Property(u => u.NormalizedEmail)
                .HasColumnName("normalized_email")
                .HasMaxLength(256);

            builder.Property(u => u.PasswordHash)
                .HasColumnName("password_hash");

            builder.Property(u => u.SecurityStamp)
                .HasColumnName("security_stamp");

            builder.Property(u => u.ConcurrencyStamp)
                .HasColumnName("concurrency_stamp");

            builder.Property(u => u.PhoneNumber)
                .HasColumnName("phone_number");

            builder.Property(u => u.PhoneNumberConfirmed)
                .HasColumnName("phone_number_confirmed");

            builder.Property(u => u.EmailConfirmed)
                .HasColumnName("email_confirmed");

            builder.Property(u => u.LockoutEnabled)
                .HasColumnName("lockout_enabled");

            builder.Property(u => u.LockoutEnd)
                .HasColumnName("lockout_end");

            builder.Property(u => u.TwoFactorEnabled)
                .HasColumnName("two_factor_enabled");

            builder.Property(u => u.AccessFailedCount)
                .HasColumnName("access_failed_count");

            builder.HasIndex(u => u.NormalizedUserName)
                .HasDatabaseName("ix_tb_oneid_users_normalized_user_name")
                .IsUnique();

            builder.HasIndex(u => u.NormalizedEmail)
                .HasDatabaseName("ix_tb_oneid_users_normalized_email");
        }
    }

}
