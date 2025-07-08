using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OneID.Domain.Entities.Sagas;

namespace OneID.Data.Mappings
{
    public sealed class SagaDeduplicationKeyMap : IEntityTypeConfiguration<SagaDeduplicationKey>
    {
        public void Configure(EntityTypeBuilder<SagaDeduplicationKey> builder)
        {
            builder.ToTable("tb_oneid_deduplication_key");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.BusinessKey)
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

            builder.HasIndex(x => new { x.BusinessKey, x.ProcessName })
                   .IsUnique()
                   .HasDatabaseName("ux_deduplication_key_business_process");
        }


    }

}
