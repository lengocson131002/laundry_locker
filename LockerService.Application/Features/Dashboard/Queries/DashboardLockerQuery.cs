using LockerService.Application.Features.Dashboard.Models;

namespace LockerService.Application.Features.Dashboard.Queries;

public class DashboardLockerQuery : IRequest<ListResponse<DashboardLockerItem>>
{
    public long? StoreId { get; set; }
}