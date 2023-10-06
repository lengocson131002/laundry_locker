using LockerService.Application.Features.Dashboard.Models;

namespace LockerService.Application.Features.Dashboard.Queries;

public class DashboardOrderQuery : IRequest<DashboardOrderResponse>
{
    public DateTimeOffset? From { get; set; }

    public DateTimeOffset? To { get; set; }
    
    public long? StoreId { get; set; }
    
    public long? LockerId { get; set; }

}