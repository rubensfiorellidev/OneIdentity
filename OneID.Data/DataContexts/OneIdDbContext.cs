using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.Logging;
using OneID.Domain.Entities;
using System.Text.Json;

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

            builder.Entity<IdentityRole>(entity =>
            {
                entity.Property(e => e.ConcurrencyStamp).IsConcurrencyToken();
            });

            var jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            builder.Entity<IdentityRole>(entity =>
            {
                entity.Property(e => e.ConcurrencyStamp).IsConcurrencyToken();
            });




        }


    }
}
