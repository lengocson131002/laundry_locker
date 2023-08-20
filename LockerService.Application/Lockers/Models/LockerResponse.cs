namespace LockerService.Application.Lockers.Models;

public class LockerResponse
{
    public long Id { get; set; }

    public string? Name { get; set; }

    public string? Code { get; set; }
    
    public string? Image { get; set; }
    
    public LockerStatus? Status { get; set; } = LockerStatus.Initialized;
    
    public LocationResponse? Location { get; set; }
    
    public string? Description { get; set; }
    
    public StoreResponse? Store { get; set; }
    
    public string? MacAddress { get; set; }
    
    public string? IpAddress { get; set; }
    
    public DateTimeOffset CreatedAt { get; set; }
    
    public long? CreatedBy { get; set; }
    
    public DateTimeOffset UpdatedAt { get; set; }
    
    public long? UpdatedBy { get; set; }
    
    public DateTimeOffset? DeletedAt { get; set; }
    
    public long? DeletedBy { get; set; }
    
    public int BoxCount { get; set; } 
}