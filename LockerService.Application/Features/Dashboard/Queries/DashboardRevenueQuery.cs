using LockerService.Application.Features.Dashboard.Models;

namespace LockerService.Application.Features.Dashboard.Queries;

public class DashboardRevenueQuery : IRequest<ListResponse<DashboardRevenueItem>>
{
    public int Year { get; set; }
    
    public long? StoreId { get; set; }
    
    public long? LockerId { get; set; }
}