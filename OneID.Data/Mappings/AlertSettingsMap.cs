using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OneID.Domain.Entities.AlertsContext;

namespace OneID.Data.Mappings
{
    internal class AlertSettingsMap : IEntityTypeConfiguration<AlertSettings>
    {
        public void Configure(EntityTypeBuilder<AlertSettings> builder)
        {
            builder
               .ToTable("tb_oneid_alert_settings");

            builder.HasKey(x => x.Id);

            builder.Property(e => e.CriticalRecipients)
                .HasColumnType("jsonb")
                .IsRequired();

            builder.Property(e => e.WarningRecipients)
                .HasColumnType("jsonb")
                .IsRequired();

            builder.Property(e => e.InfoRecipients)
                .HasColumnType("jsonb")
                .IsRequired();
        }
    }
}
