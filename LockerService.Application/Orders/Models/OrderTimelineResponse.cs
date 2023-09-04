namespace LockerService.Application.Orders.Models;

public class OrderTimelineResponse: BaseAuditableEntityResponse
{
    public long Id { get; set; }
    
    public OrderStatus Status { get; set; }
    
    public OrderStatus? PreviousStatus { get; set; }
    
    public string? Description { get; set; }
    
}