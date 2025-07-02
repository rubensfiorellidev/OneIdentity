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

            builder.Property(u => u.Fullname)
                .HasColumnName("fullname")
                .HasColumnType("VARCHAR")
                .HasMaxLength(250)
                .IsRequired();

            builder.Property(u => u.Login)
                .HasColumnName("login")
                .HasColumnType("VARCHAR")
                .HasMaxLength(150)
                .IsRequired();
        }
    }
}
