using LockerService.Application.Common.Services;
using LockerService.Domain.Enums;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace LockerService.Infrastructure.Persistence;


public class AuditableEntitySaveChangesInterceptor : SaveChangesInterceptor
{
    private readonly ICurrentAccountService _currentAccountService;
    private readonly ILogger<AuditableEntitySaveChangesInterceptor> _logger;

    public AuditableEntitySaveChangesInterceptor(
        ILogger<AuditableEntitySaveChangesInterceptor> logger, 
        ICurrentAccountService currentAccountService)
    {
        _logger = logger;
        _currentAccountService = currentAccountService;
    }

    public override async ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result,
        CancellationToken cancellationToken = new())
    {
        await UpdateEntities(eventData.Context);
        return await base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    private async Task UpdateEntities(DbContext? context)
    {
        if (context == null) return;
        var currentAccount = await _currentAccountService.GetCurrentAccount();
        var currentAccountId = currentAccount?.Id;
        var currentAccountUsername = currentAccount?.Username;
        
        var audits = new List<Audit>();
        foreach (var entry in context.ChangeTracker.Entries())
        {
            // Save audit log table
            var auditEntry = GetAuditEntry(entry);
            if (auditEntry != null)
            {
                var audit = auditEntry.ToAudit();
                
                audit.CreatedBy = currentAccountId;
                audit.CreatedByUsername = currentAccountUsername;
                audit.UpdatedBy = currentAccountId;
                audit.UpdatedByUsername = currentAccountUsername;
                
                audits.Add(audit);
            }
        }
        
        foreach (var audit in audits)
        {
            context.Add(audit);
        }
        
        foreach (var entry in context.ChangeTracker.Entries<BaseAuditableEntity>())
        {
            if (entry.State == EntityState.Added)
            {
                entry.Entity.CreatedBy = currentAccountId;
                entry.Entity.CreatedByUsername = currentAccountUsername;
                entry.Entity.CreatedAt = DateTimeOffset.UtcNow;
            }

            if (entry.State == EntityState.Added || entry.State == EntityState.Modified || entry.HasChangedOwnedEntities())
            {
                entry.Entity.UpdatedBy = currentAccountId;
                entry.Entity.UpdatedByUsername = currentAccountUsername;
                entry.Entity.UpdatedAt = DateTimeOffset.UtcNow;
            }
        }
    }

    private AuditEntry? GetAuditEntry(EntityEntry entry)
    {
        if (entry.Entity is Audit || entry.State == EntityState.Detached || entry.State == EntityState.Unchanged)
        {
            return null;
        }
        
        var auditEntry = new AuditEntry(entry);
        auditEntry.TableName = entry.Entity.GetType().Name;
        
        foreach (var property in entry.Properties)
        {
            var propertyName = property.Metadata.Name;
            if (property.Metadata.IsPrimaryKey())
            {
                auditEntry.KeyValues[propertyName] = property.CurrentValue ?? string.Empty;
                continue;
            }
            switch (entry.State)
            {
                case EntityState.Added:
                    auditEntry.AuditType = AuditType.Create;
                    auditEntry.NewValues[propertyName] = property.CurrentValue ?? string.Empty;
                    break;
                
                case EntityState.Deleted:
                    auditEntry.AuditType = AuditType.Delete;
                    auditEntry.OldValues[propertyName] = property.OriginalValue ?? string.Empty;
                    break;
                
                case EntityState.Modified:
                    if (property.IsModified)
                    {
                        auditEntry.ChangedColumns.Add(propertyName);
                        auditEntry.AuditType = AuditType.Update;
                        auditEntry.OldValues[propertyName] = property.OriginalValue ?? string.Empty;
                        auditEntry.NewValues[propertyName] = property.CurrentValue ?? string.Empty;
                    }
                    break;
            }
        }

        return auditEntry;
    }
}

public static class Extensions
{
    public static bool HasChangedOwnedEntities(this EntityEntry entry) =>
        entry.References.Any(r => 
            r.TargetEntry != null && 
            r.TargetEntry.Metadata.IsOwned() && 
            (r.TargetEntry.State == EntityState.Added || r.TargetEntry.State == EntityState.Modified));
}