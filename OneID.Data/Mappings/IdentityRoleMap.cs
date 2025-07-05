using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OneID.Domain.Entities.UserContext;

namespace OneID.Data.Mappings
{
    internal class ApplicationRoleMap : IEntityTypeConfiguration<ApplicationRole>
    {
        public void Configure(EntityTypeBuilder<ApplicationRole> builder)
        {
            builder.ToTable("tb_oneid_roles");

            builder.Property(r => r.Id);
            builder.Property(r => r.Name).HasMaxLength(256);
            builder.Property(r => r.NormalizedName).HasMaxLength(256);
            builder.Property(r => r.ConcurrencyStamp);
            builder.Property(r => r.Description).HasMaxLength(500);

            builder.HasIndex(r => r.NormalizedName)
                .HasDatabaseName("ix_tb_oneid_roles_normalized_name")
                .IsUnique();
        }
    }

}
