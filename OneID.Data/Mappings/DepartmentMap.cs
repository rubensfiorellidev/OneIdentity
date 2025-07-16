using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OneID.Domain.Entities.DepartmentContext;
using OneID.Domain.ValueObjects;

namespace OneID.Data.Mappings
{
    internal class DepartmentMap : IEntityTypeConfiguration<Department>
    {
        public void Configure(EntityTypeBuilder<Department> builder)
        {
            builder.ToTable("tb_oneid_departments");

            builder.HasKey(d => d.Id);

            builder.Property(d => d.Id)
                   .HasMaxLength(100)
                   .IsRequired();

            builder.Property(d => d.Name)
                   .HasMaxLength(200)
                   .IsRequired();

            builder.Property(d => d.Status)
                   .HasConversion(
                       status => status.Value,
                       value => DepartmentStatus.From(value))
                   .HasMaxLength(50)
                   .IsRequired();

            builder.Property(d => d.ProvisioningAt)
                   .HasColumnType("timestamptz")
                   .IsRequired();

            builder.Property(d => d.UpdatedAt)
                   .HasColumnType("timestamptz");

            builder.Property(d => d.CreatedBy)
                   .HasMaxLength(100);

            builder.Property(d => d.UpdatedBy)
                   .HasMaxLength(100);

            builder.HasIndex(d => d.Name).HasDatabaseName("idx_department_name");
        }
    }

}
