using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OneID.Domain.Entities.AuditSagas;

namespace OneID.Data.Mappings
{
    internal class AdmissionAuditMap : IEntityTypeConfiguration<AdmissionAudit>
    {
        public void Configure(EntityTypeBuilder<AdmissionAudit> builder)
        {
            builder
              .ToTable("tb_oneid_admission_audit");

            builder
                .HasKey(x => x.Id);

            builder
                .Property(a => a.FirstName)
                .IsRequired()
                .HasMaxLength(128);

            builder
                .Property(a => a.LastName)
                .IsRequired()
                .HasMaxLength(128);

            builder
                .Property(a => a.DatabaseId)
                .IsRequired()
                .HasMaxLength(100);

            builder
                .Property(a => a.CurrentState)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(x => x.EventName)
                .HasMaxLength(250)
                .IsRequired();

            builder
                .Property(a => a.ProvisioningDate)
                .HasColumnType("timestamptz")
                .IsRequired();

            builder
                .Property(a => a.Description)
                .IsRequired()
                .HasMaxLength(500);

            builder
                .Property(a => a.Login)
                .IsRequired(false)
                .HasMaxLength(100);

        }
    }
}
