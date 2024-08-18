using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;
using SharedKernel.Core;
using UrlShortener.Application.Abstractions.Services;

namespace UrlShortener.Persistence.Interceptors
{
    internal sealed class UpdateAuditableEntitiesInterceptor(ICurrentSessionProvider currentSessionProvider)
        : SaveChangesInterceptor
    {
        private const string SystemSource = "system";
        private ICurrentSessionProvider CurrentSessionProvider => currentSessionProvider;

        public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
            DbContextEventData eventData,
            InterceptionResult<int> result,
            CancellationToken cancellationToken = default)
        {
            DbContext? dbContext = eventData.Context;

            if (dbContext is null)
            {
                return base.SavingChangesAsync(
                    eventData,
                    result,
                    cancellationToken);
            }

            IEnumerable<EntityEntry<IAuditableEntity>> entries =
                dbContext
                    .ChangeTracker
                    .Entries<IAuditableEntity>();

            foreach (EntityEntry<IAuditableEntity> entityEntry in entries)
            {
                if (entityEntry.State == EntityState.Added)
                {
                    entityEntry.Property(a => a.CreatedAtUtc).CurrentValue = DateTime.UtcNow;
                    entityEntry.Property(a => a.CreatedBy).CurrentValue = CurrentSessionProvider?.GetUserId().ToString() ?? SystemSource;
                }

                if (entityEntry.State == EntityState.Modified)
                {
                    entityEntry.Property(a => a.UpdatedAtUtc).CurrentValue = DateTime.UtcNow;
                    entityEntry.Property(a => a.UpdatedBy).CurrentValue = CurrentSessionProvider?.GetUserId().ToString() ?? SystemSource;
                }
            }

            return base.SavingChangesAsync(
                eventData,
                result,
                cancellationToken);
        }
    }
}