namespace LockerService.Application.Dashboard.Queries;

public class DashboardOverviewQuery : IRequest<DashboardOverviewResponse>
{
    public DateTimeOffset? From { get; set; }

    public DateTimeOffset? To { get; set; }

}