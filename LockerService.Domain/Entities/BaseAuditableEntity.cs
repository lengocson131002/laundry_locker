using EntityFrameworkCore.Projectables;

namespace LockerService.Domain.Entities;

public class BaseAuditableEntity
{
    public DateTimeOffset CreatedAt { get; set; }
    
    public long? CreatedBy { get; set; }
    
    public string? CreatedByUsername { get; set; }
    
    public DateTimeOffset UpdatedAt { get; set; }
    
    public long? UpdatedBy { get; set; }
    
    public string? UpdatedByUsername { get; set; }
    
    public DateTimeOffset? DeletedAt { get; set; }
    
    public long? DeletedBy { get; set; }
    
    public string? DeletedByUsername { get; set; }

    [Projectable]
    public bool Deleted => DeletedAt != null;
    
}