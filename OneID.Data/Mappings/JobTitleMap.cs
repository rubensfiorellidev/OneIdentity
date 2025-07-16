using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OneID.Domain.Entities.JobTitleContext;
using OneID.Domain.ValueObjects;

namespace OneID.Data.Mappings
{
    internal class JobTitleMap : IEntityTypeConfiguration<JobTitle>
    {
        public void Configure(EntityTypeBuilder<JobTitle> builder)
        {
            builder.ToTable("tb_oneid_job_titles");

            builder.HasKey(j => j.Id);

            builder.Property(j => j.Id)
                   .HasMaxLength(100)
                   .IsRequired();

            builder.Property(j => j.Name)
                   .HasMaxLength(150)
                   .IsRequired();

            builder.Property(j => j.Status)
                   .HasConversion(
                       status => status.Value,
                       value => JobTitleStatus.From(value))
                   .HasColumnName("Status")
                   .HasMaxLength(50)
                   .IsRequired();

            builder.Property(j => j.CreatedBy)
                   .HasMaxLength(100)
                   .IsRequired();

            builder.Property(j => j.UpdatedBy)
                   .HasMaxLength(100);

            builder.Property(j => j.ProvisioningAt)
                   .HasColumnType("timestamptz")
                   .IsRequired();

            builder.Property(j => j.UpdatedAt)
                   .HasColumnType("timestamptz");

            builder.HasIndex(j => j.Name).HasDatabaseName("idx_job_title_name");
        }
    }

}
