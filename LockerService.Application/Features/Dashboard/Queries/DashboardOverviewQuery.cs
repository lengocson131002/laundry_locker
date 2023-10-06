using LockerService.Application.Features.Dashboard.Models;

namespace LockerService.Application.Features.Dashboard.Queries;

public class DashboardOverviewQuery : IRequest<DashboardOverviewResponse>
{
    public DateTimeOffset? From { get; set; }

    public DateTimeOffset? To { get; set; }

    public long? StoreId { get; set; }
}