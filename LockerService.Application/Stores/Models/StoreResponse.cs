namespace LockerService.Application.Stores.Models;

public class StoreResponse
{
    public long Id { get; set; }
    
    public string Name { get; set; } = default!;
    
    public string? ContactPhone { get; set; }
    
    public StoreStatus Status { get; set; }
    
    public LocationResponse Location { get; set; } = default!;
    
    public string? Image { get; set; }
    
    public DateTimeOffset CreatedAt { get; set; }
    
    public long? CreatedBy { get; set; }
    
    public DateTimeOffset? UpdatedAt { get; set; }
    
    public long? UpdatedBy { get; set; }
    
    public DateTimeOffset? DeletedAt { get; set; }
    
    public long? DeletedBy { get; set; }
}