namespace LockerService.Application.Lockers.Models;

public class BoxStatus
{
    public int BoxOrder { get; set; }
    
    public bool IsAvailable { get; set; }
    
    public long? OrderId { get; set; }
    
    public OrderStatus? OrderStatus { get; set; }
    
    public long? ServiceId { get; set; }
    
    public string? ServiceName { get; set; }
    
}