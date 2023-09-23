namespace LockerService.Application.Lockers.Models;

public class LockerResponse : BaseAuditableEntityResponse
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
    
    public int BoxCount { get; set; }

    public IList<OrderType> OrderTypes { get; set; } = new List<OrderType>();
}