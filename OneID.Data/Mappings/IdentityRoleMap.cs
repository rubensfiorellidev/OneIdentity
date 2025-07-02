using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OneID.Domain.Entities;

namespace OneID.Data.Mappings
{
    internal class ApplicationRoleMap : IEntityTypeConfiguration<ApplicationRole>
    {
        public void Configure(EntityTypeBuilder<ApplicationRole> builder)
        {
            builder.ToTable("tb_oneid_roles");

            builder.Property(r => r.Id).HasColumnName("id");
            builder.Property(r => r.Name).HasColumnName("name").HasMaxLength(256);
            builder.Property(r => r.NormalizedName).HasColumnName("normalized_name").HasMaxLength(256);
            builder.Property(r => r.ConcurrencyStamp).HasColumnName("concurrency_stamp");
            builder.Property(r => r.Description).HasColumnName("description");

            builder.HasIndex(r => r.NormalizedName)
                .HasDatabaseName("ix_tb_oneid_roles_normalized_name")
                .IsUnique();
        }
    }

}
