namespace LockerService.Application.Lockers.Models;

public class BoxResponse
{
    public long Id { get; set; }
    
    public int Number { get; set; }
    
    public int? PinNo { get; set; }

    public BoxStatus Status { get; set; }
    
    public long LockerId { get; set; }
    
    public long? OrderId { get; set; }
    
    public OrderType OrderType { get; set; }
    
    public OrderStatus? OrderStatus { get; set; }
    
}