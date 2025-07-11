using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OneID.Domain.Entities.Packages;

namespace OneID.Data.Mappings
{
    public class AccessPackageItemMap : IEntityTypeConfiguration<AccessPackageItem>
    {
        public void Configure(EntityTypeBuilder<AccessPackageItem> builder)
        {
            builder.ToTable("tb_oneid_access_package_items");

            builder.HasKey(i => i.Id);

            builder.Property(i => i.Id)
                .IsRequired()
                .HasMaxLength(26);

            builder.Property(i => i.AccessPackageId)
                .IsRequired()
                .HasMaxLength(26);

            builder.Property(i => i.Type)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(i => i.Value)
                .IsRequired()
                .HasMaxLength(200);
        }
    }

}
