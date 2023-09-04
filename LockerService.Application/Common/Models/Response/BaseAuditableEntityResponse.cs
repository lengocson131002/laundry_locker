namespace LockerService.Application.Common.Models.Response;

public class BaseAuditableEntityResponse
{
    public DateTimeOffset CreatedAt { get; set; }
    
    public DateTimeOffset UpdatedAt { get; set; }
    
    public DateTimeOffset? DeletedAt { get; set; }
    
    public bool Deleted => DeletedAt != null;
}