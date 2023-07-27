namespace LockerService.Application.Orders.Models;

public class OrderTimelineResponse
{
    public long Id { get; set; }
    
    public OrderStatus Status { get; set; }
    
    public OrderStatus? PreviousStatus { get; set; }
    
    public string? Description { get; set; }
    
    public DateTimeOffset? CreatedAt { get; set; }
    
    public DateTimeOffset? UpdatedAt { get; set; }
    
    public DateTimeOffset? DeletedAt { get; set; }
    
}