namespace LockerService.Application.Dashboard.Queries;

public class GetOrderDashboardRequest : IRequest<OrderDashboardResponse>
{
    public int? LockerId { get; set; }
    
    public DateTimeOffset? From { get; set; }
    
    public DateTimeOffset? To { get; set; } 
}