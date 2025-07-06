using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OneID.Domain.Entities.JwtWebTokens;

namespace OneID.Data.Mappings
{
    internal class RefreshWebTokenMap : IEntityTypeConfiguration<RefreshWebToken>
    {
        public void Configure(EntityTypeBuilder<RefreshWebToken> builder)
        {
            builder
                .ToTable("tb_oneid_refresh_web_token");

            builder
                .HasKey(x => x.Id);

            builder
                .Property(x => x.Jti)
                .HasColumnType("VARCHAR")
                .HasMaxLength(500);

            builder
               .Property(x => x.Token)
               .HasColumnType("VARCHAR")
               .HasMaxLength(500);

        }
    }
}
