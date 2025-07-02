using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.Logging;
using OneID.Data.Mappings;
using OneID.Domain.Entities;

namespace OneID.Data.DataContexts
{
    public sealed class OneIdDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, string>
    {
        public OneIdDbContext() { }

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

            builder.Entity<ApplicationRole>(entity =>
            {
                entity.ToTable("tb_oneid_roles");
                entity.Property(e => e.ConcurrencyStamp).IsConcurrencyToken();
            });

        }
    }

}
