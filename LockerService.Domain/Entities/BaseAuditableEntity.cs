using System.ComponentModel.DataAnnotations;

namespace LockerService.Domain.Entities;

public class BaseAuditableEntity
{
    public DateTimeOffset CreatedAt { get; set; }
    
    public long? CreatedBy { get; set; }
    
    public DateTimeOffset UpdatedAt { get; set; }
    
    public long? UpdatedBy { get; set; }
    
    public DateTimeOffset? DeletedAt { get; set; }
    
    public long? DeletedBy { get; set; }
}