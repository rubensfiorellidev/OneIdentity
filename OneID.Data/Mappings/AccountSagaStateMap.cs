using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OneID.Application.Messaging.Sagas.Contracts;

namespace OneID.Data.Mappings
{
    internal class AccountSagaStateMap : IEntityTypeConfiguration<AccountSagaState>
    {
        public void Configure(EntityTypeBuilder<AccountSagaState> builder)
        {
            builder
               .ToTable("tb_oneid_account_saga_state");

            builder.HasKey(x => x.CorrelationId);

            builder.Property(x => x.CorrelationId)
                    .ValueGeneratedNever();

            builder.Property(x => x.CurrentState)
                    .IsRequired()
                    .HasMaxLength(50);

            builder.Property(x => x.FaultReason)
                    .HasMaxLength(500);

            builder.Property(x => x.Version)
                    .IsRowVersion()
                    .IsRequired(false);

            builder.Property(x => x.CreatedAt)
                    .HasColumnType("timestamptz")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP AT TIME ZONE 'UTC'")
                    .IsRequired();

            builder.OwnsOne(x => x.KeycloakData, payload =>
            {
                payload.ToJson();

            });

            builder.OwnsOne(x => x.AccountData, payload =>
            {
                payload.ToJson();

            });

        }
    }
}
