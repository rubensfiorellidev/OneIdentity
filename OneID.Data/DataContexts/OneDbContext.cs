using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.Logging;
using OneID.Application.DTOs.Admission;
using OneID.Application.Messaging.Sagas.Contracts;
using OneID.Domain.Abstractions.EventsContext;
using OneID.Domain.Entities.AlertsContext;
using OneID.Domain.Entities.AuditSagas;
using OneID.Domain.Entities.JwtWebTokens;
using OneID.Domain.Entities.Sagas;
using OneID.Domain.Entities.UserContext;
using OneID.Domain.Notifications;

namespace OneID.Data.DataContexts
{
    public sealed class OneDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, string>
    {
        public OneDbContext() { }

        public DbSet<AccountSagaState> AccountSagaStates => Set<AccountSagaState>();
        public DbSet<AdmissionAudit> AdmissionAudits => Set<AdmissionAudit>();
        public DbSet<RefreshWebToken> RefreshWebTokens => Set<RefreshWebToken>();
        public DbSet<StoredEvent> StoredEvents => Set<StoredEvent>();
        public DbSet<UserAccount> UserAccounts => Set<UserAccount>();
        public DbSet<SagaDeduplicationKey> SagaDeduplicationKeys => Set<SagaDeduplicationKey>();
        public DbSet<SagaDeduplication> SagaDeduplications => Set<SagaDeduplication>();
        public DbSet<AccountAdmissionStaging> AccountAdmissionStagings => Set<AccountAdmissionStaging>();
        public DbSet<AlertSettings> AlertSettings => Set<AlertSettings>();






        public OneDbContext(DbContextOptions<OneDbContext> options)
            : base(options)
        {
            ChangeTracker.LazyLoadingEnabled = false;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);

            optionsBuilder.UseLoggerFactory(LoggerFactory.Create(builder =>
            {
                builder.AddConsole();
            })).EnableSensitiveDataLogging();

            if (!optionsBuilder.IsConfigured)
            {
                throw new InvalidOperationException("DbContext was not configured. Ensure AddDbContext is called in the DI configuration.");
            }
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);


            builder.ApplyConfigurationsFromAssembly(typeof(OneDbContext).Assembly);


            builder.Entity<ApplicationRole>(entity =>
            {
                entity.ToTable("tb_oneid_roles");
                entity.Property(e => e.ConcurrencyStamp).IsConcurrencyToken();
            });

            builder.Entity<IdentityUserRole<string>>(entity =>
            {
                entity.ToTable("tb_oneid_user_roles");
            });

            builder.Entity<IdentityUserClaim<string>>().ToTable("tb_oneid_user_claims");
            builder.Entity<IdentityUserLogin<string>>().ToTable("tb_oneid_user_logins");
            builder.Entity<IdentityRoleClaim<string>>().ToTable("tb_oneid_role_claims");
            builder.Entity<IdentityUserToken<string>>().ToTable("tb_oneid_user_tokens");

            builder.Ignore<Notification>();
            builder.Ignore<Event>();
            builder.Ignore<UserAccountPayload>();

        }
    }

}
