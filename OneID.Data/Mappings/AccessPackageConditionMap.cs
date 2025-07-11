using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OneID.Domain.Entities.Packages;
#nullable disable
namespace OneID.Data.Mappings
{
    public class AccessPackageConditionMap : IEntityTypeConfiguration<AccessPackageCondition>
    {
        public void Configure(EntityTypeBuilder<AccessPackageCondition> builder)
        {
            builder.ToTable("tb_oneid_access_package_condition");

            builder.HasKey(c => c.Id);

            builder.Property(c => c.Id)
                .IsRequired()
                .HasMaxLength(26);

            builder.Property(c => c.AccessPackageId)
                .IsRequired()
                .HasMaxLength(26);

            builder.Property(c => c.Department)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(c => c.JobTitle)
                .IsRequired()
                .HasMaxLength(100);

            builder.HasOne(c => c.AccessPackage)
                   .WithMany()
                   .HasForeignKey(c => c.AccessPackageId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }

}
