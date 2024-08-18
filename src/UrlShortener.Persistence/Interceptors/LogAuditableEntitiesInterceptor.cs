using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;
using SharedKernel.Core;
using UrlShortener.Application.Abstractions.Services;
using UrlShortener.Persistence.Models;

namespace UrlShortener.Persistence.Interceptors
{
    /// <summary>
    /// 審計實體攔截器
    /// </summary>
    internal sealed class LogAuditableEntitiesInterceptor : SaveChangesInterceptor
    {
        private readonly ICurrentSessionProvider _currentSessionProvider;

        public LogAuditableEntitiesInterceptor(ICurrentSessionProvider currentSessionProvider)
        {
            _currentSessionProvider = currentSessionProvider;
        }

        /// <summary>
        /// 當保存更改時觸發，記錄審計條目
        /// </summary>
        public override async ValueTask<InterceptionResult<int>> SavingChangesAsync(
            DbContextEventData eventData,
            InterceptionResult<int> result,
            CancellationToken cancellationToken = default)
        {
            var dbContext = eventData.Context;
            if (dbContext == null) return await base.SavingChangesAsync(eventData, result, cancellationToken);

            var userId = _currentSessionProvider.GetUserId();

            // 獲取所有需要審計的實體
            var auditableEntries = dbContext.ChangeTracker.Entries<IAuditableEntity>()
                .Where(entry => entry.State is EntityState.Added or EntityState.Deleted or EntityState.Modified)
                .Select(entry => CreateTrailEntry(userId, entry))
                .ToList();

            // 如果有審計條目，將其添加到上下文中
            if (auditableEntries.Any())
            {
                await dbContext.AddRangeAsync(auditableEntries, cancellationToken);
            }

            return await base.SavingChangesAsync(eventData, result, cancellationToken);
        }

        /// <summary>
        /// 創建審計條目
        /// </summary>
        private static AuditTrail CreateTrailEntry(Guid? userId, EntityEntry<IAuditableEntity> entry)
        {
            var trailEntry = new AuditTrail
            {
                Id = Guid.NewGuid(),
                EntityName = entry.Entity.GetType().Name,
                UserId = userId,
                DateAtUtc = DateTime.UtcNow
            };

            // 設置審計條目的屬性值
            SetAuditTrailPropertyValues(entry, trailEntry);
            // 設置審計條目的引用
            SetAuditTrailReferences(entry, trailEntry);
            return trailEntry;
        }

        /// <summary>
        /// 設置審計條目的屬性值
        /// </summary>
        private static void SetAuditTrailPropertyValues(EntityEntry<IAuditableEntity> entry, AuditTrail trailEntry)
        {
            foreach (var property in entry.Properties.Where(p => !p.IsTemporary))
            {
                if (property.Metadata.IsPrimaryKey())
                {
                    trailEntry.PrimaryKey = property.CurrentValue?.ToString();
                    continue;
                }

                if (property.Metadata.Name == "PasswordHash")
                {
                    // 若為 PasswordHash，改為 mask 顯示
                    trailEntry.OldValues[property.Metadata.Name] = "********";
                    trailEntry.NewValues[property.Metadata.Name] = "********";
                    continue;
                }

                // 設置屬性審計值
                SetPropertyAuditValues(entry, trailEntry, property);
            }
        }

        /// <summary>
        /// 設置屬性審計值
        /// </summary>
        private static void SetPropertyAuditValues(EntityEntry<IAuditableEntity> entry, AuditTrail trailEntry, PropertyEntry property)
        {
            var propertyName = property.Metadata.Name;

            switch (entry.State)
            {
                case EntityState.Added:
                    trailEntry.TrailType = TrailType.Create;
                    trailEntry.NewValues[propertyName] = property.CurrentValue;
                    break;

                case EntityState.Deleted:
                    trailEntry.TrailType = TrailType.Delete;
                    trailEntry.OldValues[propertyName] = property.OriginalValue;
                    break;

                case EntityState.Modified when property.IsModified && !Equals(property.OriginalValue, property.CurrentValue):
                    trailEntry.ChangedColumns.Add(propertyName);
                    trailEntry.OldValues[propertyName] = property.OriginalValue;
                    trailEntry.NewValues[propertyName] = property.CurrentValue;
                    trailEntry.TrailType = TrailType.Update;
                    break;
            }
        }

        /// <summary>
        /// 設置審計條目的引用
        /// </summary>
        private static void SetAuditTrailReferences(EntityEntry<IAuditableEntity> entry, AuditTrail trailEntry)
        {
            var changedColumns = entry.References
                .Where(r => r.IsModified)
                .Select(r => r.Metadata.Name)
                .Concat(entry.Navigations
                    .Where(n => n.Metadata.IsCollection && n.IsModified && (n.CurrentValue as IEnumerable<object>)?.Any() == true)
                    .Select(n => n.Metadata.Name))
                .ToList();

            trailEntry.ChangedColumns.AddRange(changedColumns);
        }
    }
}