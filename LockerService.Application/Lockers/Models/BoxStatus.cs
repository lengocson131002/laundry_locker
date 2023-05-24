namespace LockerService.Application.Lockers.Models;

public class BoxStatus
{
    public int BoxOrder { get; set; }
    
    public bool IsAvailable { get; set; }
    
    public int? OrderId { get; set; }
    
    public OrderStatus? OrderStatus { get; set; }
    
}