using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.Logging;
using OneID.Data.Mappings;
using OneID.Domain.Abstractions.Events;
using OneID.Domain.Entities;
using OneID.Domain.Entities.UserContext;

namespace OneID.Data.DataContexts
{
    public sealed class OneIdDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, string>
    {
        public OneIdDbContext() { }

        public DbSet<UserProfile> UsersProfile => Set<UserProfile>();

        public OneIdDbContext(DbContextOptions<OneIdDbContext> options)
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

            builder.ApplyConfiguration(new ApplicationUserMap());
            builder.ApplyConfiguration(new ApplicationRoleMap());
            builder.ApplyConfiguration(new UserProfileMap());

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

            builder.Entity<UserProfile>().Ignore(a => a.Notifications);
            builder.Ignore<Event>();

        }
    }

}
