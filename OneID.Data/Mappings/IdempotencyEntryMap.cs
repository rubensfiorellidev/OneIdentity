using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OneID.Domain.Entities.Logins;

namespace OneID.Data.Mappings
{
    internal sealed class IdempotencyEntryMap : IEntityTypeConfiguration<IdempotencyEntry>
    {
        public void Configure(EntityTypeBuilder<IdempotencyEntry> builder)
        {
            builder.ToTable("tb_oneid_idempotency");
            builder.HasKey(x => x.Key);
            builder.Property(x => x.Key).HasMaxLength(256);
            builder.Property(x => x.Payload).HasColumnType("jsonb");
            builder.Property(x => x.CreatedAtUtc).HasColumnType("timestamptz");
        }
    }
}
