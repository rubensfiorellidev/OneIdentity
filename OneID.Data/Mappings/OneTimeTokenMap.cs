using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OneID.Domain.Entities.Tokens;

namespace OneID.Data.Mappings
{
    public class OneTimeTokenMap : IEntityTypeConfiguration<OneTimeToken>
    {
        public void Configure(EntityTypeBuilder<OneTimeToken> builder)
        {
            builder.ToTable("tb_oneid_tokens");

            builder.HasKey(t => t.Jti);

            builder.Property(t => t.Jti)
                .HasMaxLength(64)
                .IsRequired();

            builder.Property(t => t.CorrelationId)
                .HasMaxLength(64)
                .IsRequired();

            builder.Property(t => t.ExpiresAt)
                .IsRequired();

            builder.Property(t => t.Used)
                .IsRequired();
        }
    }

}
