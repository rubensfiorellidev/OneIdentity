using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OneID.Domain.Abstractions.EventsContext;

namespace OneID.Data.Mappings
{
    internal class StoredEventMap : IEntityTypeConfiguration<StoredEvent>
    {
        public void Configure(EntityTypeBuilder<StoredEvent> builder)
        {
            builder.ToTable("tb_oneid_stored_events");

            builder.HasKey(e => e.Id);
            builder.Property(e => e.Id).HasMaxLength(100).IsRequired();
            builder.Property(e => e.AggregateId).HasMaxLength(100).IsRequired();
            builder.Property(e => e.AggregateType).HasMaxLength(100).IsRequired();
            builder.Property(e => e.EventType).HasMaxLength(150).IsRequired();
            builder.Property(e => e.EventData).HasColumnType("jsonb").IsRequired();
            builder.Property(e => e.OccurredOn).HasColumnType("timestamptz").IsRequired();
            builder.Property(e => e.Version).IsRequired();
            builder.Property(e => e.CreatedBy).HasMaxLength(100).IsRequired();
            builder.Property(e => e.Description).HasMaxLength(500).IsRequired(false);
        }
    }
}
