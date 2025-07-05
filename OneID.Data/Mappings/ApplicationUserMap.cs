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
                .HasColumnType("varchar")
                .HasMaxLength(26)
                .IsRequired();

            builder.Property(u => u.Fullname)
                .HasColumnType("varchar")
                .HasMaxLength(250)
                .IsRequired();

            builder.Property(u => u.FirstName)
                .HasColumnType("varchar")
                .HasMaxLength(250)
                .IsRequired();

            builder.Property(u => u.LastName)
                .HasColumnType("varchar")
                .HasMaxLength(250)
                .IsRequired();

            builder.Property(u => u.LoginHash)
                .HasColumnType("varchar")
                .HasMaxLength(150)
                .IsRequired();

            builder.Property(u => u.LoginCrypt)
                .HasColumnType("varchar")
                .HasMaxLength(250)
                .IsRequired();

            builder.Property(u => u.ProvisioningAt)
                .HasColumnType("timestamptz")
                .HasDefaultValueSql("CURRENT_TIMESTAMP AT TIME ZONE 'UTC'")
                .IsRequired();

            builder.Property(u => u.IsActive)
                .HasColumnType("boolean")
                .HasDefaultValue(true)
                .IsRequired();

            builder.Property(u => u.LastLoginAt)
                .HasColumnType("timestamptz")
                .IsRequired(false);

            builder.Property(u => u.CreatedBy)
                .HasColumnType("varchar")
                .HasMaxLength(100)
                .IsRequired(false);

            builder.Property(u => u.UserName)
                .HasMaxLength(256);

            builder.Property(u => u.NormalizedUserName)
                .HasMaxLength(256);

            builder.Property(u => u.Email)
                .HasMaxLength(256);

            builder.Property(u => u.NormalizedEmail)
                .HasMaxLength(256);

            builder.Property(u => u.PasswordHash);

            builder.Property(u => u.SecurityStamp);

            builder.Property(u => u.ConcurrencyStamp);

            builder.Property(u => u.PhoneNumber);

            builder.Property(u => u.PhoneNumberConfirmed);

            builder.Property(u => u.EmailConfirmed);

            builder.Property(u => u.LockoutEnabled);

            builder.Property(u => u.LockoutEnd);

            builder.Property(u => u.TwoFactorEnabled);

            builder.Property(u => u.AccessFailedCount);

            builder.HasIndex(u => u.NormalizedUserName)
                .HasDatabaseName("ix_tb_oneid_users_normalized_user_name")
                .IsUnique();

            builder.HasIndex(u => u.NormalizedEmail)
                .HasDatabaseName("ix_tb_oneid_users_normalized_email");
        }
    }

}
