using LockerService.Application.Features.Staffs.Models;

namespace LockerService.Application.Features.Orders.Models;

public class OrderTimelineResponse: BaseAuditableEntityResponse
{
    public long Id { get; set; }
    
    public OrderStatus Status { get; set; }
    
    public OrderStatus? PreviousStatus { get; set; }
    
    public string? Description { get; set; }
 
    public StaffResponse? Staff { get; set; }
    
    public string? Image { get; set; }
}