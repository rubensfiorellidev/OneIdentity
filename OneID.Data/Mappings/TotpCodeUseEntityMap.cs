using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OneID.Domain.Entities.TotpOptions;

namespace OneID.Data.Mappings
{
    internal class TotpCodeUseEntityMap : IEntityTypeConfiguration<TotpCodeUseEntity>
    {
        public void Configure(EntityTypeBuilder<TotpCodeUseEntity> builder)
        {
            builder.ToTable("tb_oneid_totp_code_used_window");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.UserId).IsRequired();
            builder.Property(x => x.Step).IsRequired();
            builder.Property(x => x.CreatedAt).HasColumnType("timestamptz");
            builder.Property(x => x.ExpiresAt).HasColumnType("timestamptz");

            builder.HasIndex(x => new { x.UserId, x.Step })
                   .IsUnique()
                   .HasDatabaseName("ix_totp_code_use_user_step");

            builder.HasIndex(x => x.ExpiresAt)
                   .HasDatabaseName("ix_totp_code_use_expires_at");
        }
    }
}
