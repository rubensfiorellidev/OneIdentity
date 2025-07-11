using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OneID.Domain.Entities.Packages;

namespace OneID.Data.Mappings
{
    public class AccessPackageMap : IEntityTypeConfiguration<AccessPackage>
    {
        public void Configure(EntityTypeBuilder<AccessPackage> builder)
        {
            builder.ToTable("tb_oneid_access_package");

            builder.HasKey(p => p.Id);

            builder.Property(p => p.Id)
                .IsRequired()
                .HasMaxLength(26);

            builder.Property(p => p.Name)
                .IsRequired()
                .HasMaxLength(150);

            builder.Property(p => p.CreatedBy)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(p => p.UpdatedBy)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(p => p.ProvisioningAt)
                .IsRequired();

            builder.Property(p => p.UpdatedAt)
                 .HasColumnType("timestamptz")
                 .IsRequired();

            builder.HasMany(p => p.Items)
                   .WithOne(i => i.AccessPackage)
                   .HasForeignKey(i => i.AccessPackageId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }

}
