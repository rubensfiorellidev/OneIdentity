using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.Logging;
using OneID.Application.Messaging.Sagas.Contracts;
using OneID.Domain.Abstractions.EventsContext;
using OneID.Domain.Entities.AlertsContext;
using OneID.Domain.Entities.AuditSagas;
using OneID.Domain.Entities.DepartmentContext;
using OneID.Domain.Entities.JobTitleContext;
using OneID.Domain.Entities.JwtWebTokens;
using OneID.Domain.Entities.Packages;
using OneID.Domain.Entities.Sagas;
using OneID.Domain.Entities.Tokens;
using OneID.Domain.Entities.UserContext;
using OneID.Domain.Notifications;
using OneID.Domain.ValueObjects;

namespace OneID.Data.DataContexts
{
    public sealed class OneDbContext : DbContext
    {
        public OneDbContext() { }

        public OneDbContext(DbContextOptions<OneDbContext> options)
            : base(options)
        {
            ChangeTracker.LazyLoadingEnabled = false;
        }

        public DbSet<UserAccount> UserAccounts => Set<UserAccount>();
        public DbSet<Role> Roles => Set<Role>();
        public DbSet<UserRole> UserRoles => Set<UserRole>();
        public DbSet<UserClaim> UserClaims => Set<UserClaim>();
        public DbSet<AccountSagaState> AccountSagaStates => Set<AccountSagaState>();
        public DbSet<AdmissionAudit> AdmissionAudits => Set<AdmissionAudit>();
        public DbSet<RefreshWebToken> RefreshWebTokens => Set<RefreshWebToken>();
        public DbSet<StoredEvent> StoredEvents => Set<StoredEvent>();
        public DbSet<SagaDeduplicationKey> SagaDeduplicationKeys => Set<SagaDeduplicationKey>();
        public DbSet<SagaDeduplication> SagaDeduplications => Set<SagaDeduplication>();
        public DbSet<AccountAdmissionStaging> AccountAdmissionStagings => Set<AccountAdmissionStaging>();
        public DbSet<AlertSettings> AlertSettings => Set<AlertSettings>();
        public DbSet<ServiceUser> ServiceUsers => Set<ServiceUser>();
        public DbSet<ServiceUserClaim> ServiceUserClaims => Set<ServiceUserClaim>();
        public DbSet<AccessPackage> AccessPackages => Set<AccessPackage>();
        public DbSet<AccessPackageItem> AccessPackageItems => Set<AccessPackageItem>();
        public DbSet<AccessPackageCondition> AccessPackageConditions => Set<AccessPackageCondition>();
        public DbSet<Department> Departments => Set<Department>();
        public DbSet<JobTitle> JobTitles => Set<JobTitle>();
        public DbSet<OneTimeToken> OneTimeTokens => Set<OneTimeToken>();




        private static void IgnoreValueObjects(ModelBuilder builder)
        {
            builder.Ignore<Notification>();
            builder.Ignore<Event>();
            builder.Ignore<TypeUserAccount>();
            builder.Ignore<UserAccountStatus>();

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

            IgnoreValueObjects(builder);

            builder.ApplyConfigurationsFromAssembly(typeof(OneDbContext).Assembly);

            builder.Entity<RefreshWebToken>()
                .Ignore(x => x.RawToken);

            // Mapeamento de UserRole
            builder.Entity<UserRole>(entity =>
            {
                entity.ToTable("tb_oneid_user_roles");

                entity.HasKey(x => new { x.UserAccountId, x.RoleId });

                entity.HasOne<UserAccount>()
                    .WithMany()
                    .HasForeignKey(x => x.UserAccountId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(x => x.Role)
                      .WithMany()
                      .HasForeignKey(x => x.RoleId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // Mapeamento de Role
            builder.Entity<Role>(entity =>
            {
                entity.ToTable("tb_oneid_roles");

                entity.HasKey(x => x.Id);

                entity.Property(x => x.Id)
                      .HasMaxLength(100)
                      .IsRequired()
                      .ValueGeneratedNever();

                entity.Property(x => x.Name)
                      .HasMaxLength(100)
                      .IsRequired();

                entity.Property(x => x.Description)
                      .HasMaxLength(250);

                entity.Property(r => r.IsActive)
                       .IsRequired();

                entity.Property(x => x.ProvisioningAt)
                      .IsRequired();

                entity.Property(x => x.UpdatedAt);

                entity.Property(x => x.CreatedBy)
                      .HasMaxLength(100);

                entity.Property(x => x.UpdatedBy)
                      .HasMaxLength(100);

            });

            // Mapeamento de UserClaim
            builder.Entity<UserClaim>(entity =>
            {
                entity.ToTable("tb_oneid_user_claims");

                entity.HasKey(x => x.Id);

                entity.Property(x => x.Id)
                      .HasMaxLength(100)
                      .IsRequired()
                      .ValueGeneratedNever();

                entity.Property(x => x.Type)
                      .HasMaxLength(100)
                      .IsRequired();

                entity.Property(x => x.Value)
                      .HasMaxLength(250)
                      .IsRequired();

                entity.HasOne(x => x.User)
                      .WithMany()
                      .HasForeignKey(x => x.UserAccountId)
                      .HasConstraintName("FK_UserClaims_UserAccounts")
                      .OnDelete(DeleteBehavior.Cascade);

            });

            // Mapeamento de ServiceUserClaim
            builder.Entity<ServiceUser>(entity =>
            {
                entity.ToTable("tb_oneid_service_user");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnType("text");
                entity.Property(e => e.Name).IsRequired();
                entity.Property(e => e.Description);
                entity.Property(e => e.IsActive);
                entity.Property(e => e.CreatedAt);

                entity.HasMany(e => e.Claims)
                      .WithOne()
                      .HasForeignKey(c => c.ServiceUserId);
            });

            builder.Entity<ServiceUserClaim>(entity =>
            {
                entity.ToTable("tb_oneid_service_user_claims");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnType("text");
                entity.Property(e => e.ServiceUserId).IsRequired();
                entity.Property(e => e.Type).IsRequired();
                entity.Property(e => e.Value).IsRequired();
            });

        }
    }

}
