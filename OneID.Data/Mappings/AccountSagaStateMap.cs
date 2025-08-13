using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OneID.Application.Messaging.Sagas.Contracts;

namespace OneID.Data.Mappings
{
    internal class AccountSagaStateMap : IEntityTypeConfiguration<AccountSagaState>
    {
        public void Configure(EntityTypeBuilder<AccountSagaState> builder)
        {
            builder.ToTable("tb_oneid_account_saga_state");

            builder.HasKey(x => x.CorrelationId);

            builder.Property(x => x.CorrelationId).ValueGeneratedNever();

            builder.Property(x => x.Version)
                    .IsConcurrencyToken()
                    .ValueGeneratedOnAddOrUpdate();

            builder.Property(x => x.CurrentState)
                    .IsRequired()
                    .HasMaxLength(64);

            builder.Property(x => x.FaultReason)
                    .HasMaxLength(1000);

            builder.Property(x => x.CreatedAt)
                    .HasColumnType("timestamptz")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP AT TIME ZONE 'UTC'")
                    .IsRequired();

            builder.Property(x => x.UpdatedAt)
                    .HasColumnType("timestamptz");

            builder.Property(x => x.LastEvent)
                    .HasMaxLength(128);
            // Login
            builder.Property(x => x.LoginAllocated).HasDefaultValue(false);
            builder.Property(x => x.Login).HasMaxLength(120);
            builder.Property(x => x.CorporateEmail).HasMaxLength(256);

            // Keycloak
            builder.Property(x => x.KeycloakCreated).HasDefaultValue(false);
            builder.Property(x => x.KeycloakUserId).HasMaxLength(128);

            // Banco OneID
            builder.Property(x => x.DatabaseCreated).HasDefaultValue(false);
            builder.Property(x => x.DatabaseId).HasMaxLength(64);

            // Entra ID
            builder.Property(x => x.AzureCreated).HasDefaultValue(false);
            builder.Property(x => x.AzureUserId).HasMaxLength(128);

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
