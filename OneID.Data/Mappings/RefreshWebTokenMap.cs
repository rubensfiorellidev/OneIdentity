using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OneID.Domain.Entities.JwtWebTokens;

namespace OneID.Data.Mappings
{
    internal class RefreshWebTokenMap : IEntityTypeConfiguration<RefreshWebToken>
    {
        public void Configure(EntityTypeBuilder<RefreshWebToken> builder)
        {
            builder.ToTable("tb_oneid_refresh_web_token");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.UserUpnHash)
               .HasColumnType("VARCHAR")
               .IsRequired();

            builder.Property(x => x.Jti)
                .HasColumnType("VARCHAR")
                .HasMaxLength(500)
                .IsRequired();

            builder.Property(x => x.TokenHash)
                .HasColumnType("VARCHAR")
                .HasMaxLength(500)
                .IsRequired();

            builder.Property(x => x.CreatedAt)
                .HasColumnType("timestamptz")
                .IsRequired();

            builder.Property(x => x.ExpiresAt)
                .HasColumnType("timestamptz")
                .IsRequired();

            builder.Property(x => x.IsRevoked)
                .IsRequired();

            builder.Property(x => x.IsUsed)
                .IsRequired();

            builder.Property(x => x.TokenSalt)
                .HasColumnType("VARCHAR")
                .HasMaxLength(100);

            builder.Property(x => x.IpAddress)
                .HasColumnType("VARCHAR")
                .HasMaxLength(100);

            builder.Property(x => x.UserAgent)
                .HasColumnType("VARCHAR")
                .HasMaxLength(1000);
        }
    }
}
