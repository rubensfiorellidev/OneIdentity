using OneID.Application.Messaging.Sagas.Contracts;

namespace OneID.Application.Services
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Diagnostics;

    public sealed class AuditStampInterceptor : SaveChangesInterceptor
    {
        public override InterceptionResult<int> SavingChanges(
            DbContextEventData e, InterceptionResult<int> result)
        {
            Stamp(e.Context);
            return base.SavingChanges(e, result);
        }

        public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
            DbContextEventData e, InterceptionResult<int> result, CancellationToken ct = default)
        {
            Stamp(e.Context);
            return base.SavingChangesAsync(e, result, ct);
        }

        private static void Stamp(DbContext? ctx)
        {
            if (ctx is null) return;

            var now = DateTimeOffset.UtcNow;
            foreach (var entry in ctx.ChangeTracker.Entries<AccountSagaState>())
            {
                if (entry.State == EntityState.Modified)
                    entry.Entity.UpdatedAt = now;

                if (entry.State == EntityState.Added && entry.Entity.CreatedAt == default)
                    entry.Entity.CreatedAt = now;
            }
        }
    }

}
