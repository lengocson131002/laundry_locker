using Microsoft.EntityFrameworkCore.Diagnostics;

namespace LockerService.Infrastructure.Persistence;

public class SoftDeleteInterceptor : SaveChangesInterceptor
{
    
    public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
    {
        UpdateEntities(eventData.Context);
        return base.SavingChanges(eventData, result);
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result,
        CancellationToken cancellationToken = new CancellationToken())
    {
        UpdateEntities(eventData.Context);
        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    private void UpdateEntities(DbContext? dbContext)
    {
        if (dbContext == null)
        {
            return;
        }
        
        foreach (var entry in dbContext.ChangeTracker.Entries())
        {
            if (entry is not { State: EntityState.Deleted, Entity: BaseAuditableEntity delete })
            {
                continue;
            }
            
            entry.State = EntityState.Modified;
            delete.DeletedAt = DateTimeOffset.UtcNow;
        }
    } 
}