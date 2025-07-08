using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OneID.Domain.Entities.Sagas;

namespace OneID.Data.Mappings
{
    internal class SagaDeduplicationMap : IEntityTypeConfiguration<SagaDeduplication>
    {
        public void Configure(EntityTypeBuilder<SagaDeduplication> builder)
        {
            builder.ToTable("tb_oneid_deduplication");

            builder.HasKey(x => x.CorrelationId);

            builder.Property(x => x.CorrelationId)
                .HasColumnType("VARCHAR")
                .HasMaxLength(300)
                .IsRequired();

            builder.Property(x => x.ProcessName)
                .HasColumnType("VARCHAR")
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(x => x.CreatedAt)
                .HasColumnType("timestamptz")
                .HasDefaultValueSql("CURRENT_TIMESTAMP AT TIME ZONE 'UTC'")
                .IsRequired();

            builder.HasIndex(x => new { x.CorrelationId, x.ProcessName })
                .IsUnique()
                .HasDatabaseName("ux_deduplication_key_business_process");

        }
    }
}
