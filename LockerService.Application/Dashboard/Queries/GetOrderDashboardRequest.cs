namespace LockerService.Application.Dashboard.Queries;

public class GetOrderDashboardRequest : IRequest<OrderDashboardResponse>
{
    public long? LockerId { get; set; }
    
    public DateTimeOffset? From { get; set; }
    
    public DateTimeOffset? To { get; set; } 
}