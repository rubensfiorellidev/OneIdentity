using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OneID.Domain.Entities.Logins;

namespace OneID.Data.Mappings
{
    internal sealed class LoginReservationMap : IEntityTypeConfiguration<LoginReservation>
    {
        public void Configure(EntityTypeBuilder<LoginReservation> builder)
        {
            builder.ToTable("tb_oneid_login_reservations");

            builder.HasKey(x => x.Login);
            builder.Property(x => x.Login).HasMaxLength(120);
            builder.Property(x => x.Status).IsRequired().HasMaxLength(16);
            builder.Property(x => x.CorrelationId).IsRequired();
            builder.HasIndex(x => x.CorrelationId).IsUnique();
            builder.Property(x => x.ReservedAtUtc).HasColumnType("timestamptz");
            builder.Property(x => x.UpdatedAtUtc).HasColumnType("timestamptz");
            builder.HasIndex(x => x.Status);
        }
    }
}
