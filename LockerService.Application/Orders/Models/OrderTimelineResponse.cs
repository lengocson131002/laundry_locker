namespace LockerService.Application.Orders.Models;

public class OrderTimelineResponse
{
    public long Id { get; set; }
    
    public OrderStatus Status { get; set; }
    
    public OrderStatus? PreviousStatus { get; set; }
    
    public DateTimeOffset Time { get; set; }
}