using EntityFrameworkCore.Projectables;

namespace LockerService.Domain.Entities;

public class BaseAuditableEntity
{
    public DateTimeOffset CreatedAt { get; set; }
    
    public long? CreatedBy { get; set; }
    
    public DateTimeOffset UpdatedAt { get; set; }
    
    public long? UpdatedBy { get; set; }
    
    public DateTimeOffset? DeletedAt { get; set; }
    
    public long? DeletedBy { get; set; }

    [Projectable]
    public bool Deleted => DeletedAt != null;
}