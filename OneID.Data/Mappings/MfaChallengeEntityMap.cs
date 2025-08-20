using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OneID.Domain.Entities.Logins;

namespace OneID.Data.Mappings
{
    internal class MfaChallengeEntityMap : IEntityTypeConfiguration<MfaChallengeEntity>
    {
        public void Configure(EntityTypeBuilder<MfaChallengeEntity> builder)
        {
            builder.ToTable("tb_oneid_mfa_challenge");

            builder.HasKey(x => x.Jti);

            builder.Property(x => x.UserId).IsRequired();
            builder.Property(x => x.CodeChallenge).IsRequired();
            builder.Property(x => x.IpHash).IsRequired();
            builder.Property(x => x.UserAgentHash).IsRequired();

            builder.Property(x => x.ExpiresAt).HasColumnType("timestamptz");
            builder.Property(x => x.CreatedAt).HasColumnType("timestamptz");
            builder.Property(x => x.ConsumedAt).HasColumnType("timestamptz");

            builder.Property(x => x.RowVersion)
                   .IsRowVersion()
                   .IsConcurrencyToken();

            builder.HasIndex(x => x.UserId)
                   .HasDatabaseName("ix_mfa_challenge_user_id");

            builder.HasIndex(x => x.ExpiresAt)
                   .HasDatabaseName("ix_mfa_challenge_expires_at");

            builder.HasIndex(x => x.Used)
                   .HasDatabaseName("ix_mfa_challenge_used");

            builder.HasIndex(x => new { x.UserId, x.Used, x.ExpiresAt })
                   .HasDatabaseName("ix_mfa_challenge_user_used_expires");

            builder.HasIndex(x => x.ExpiresAt)
                   .HasFilter("\"Used\" = FALSE")
                   .HasDatabaseName("ix_mfa_challenge_active_by_expiry");


            builder.HasIndex(x => new { x.UserId, x.CodeChallenge }).IsUnique();
        }
    }
}
