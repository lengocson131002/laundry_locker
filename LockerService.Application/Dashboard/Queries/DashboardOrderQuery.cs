namespace LockerService.Application.Dashboard.Queries;

public class DashboardOrderQuery : IRequest<DashboardOrderResponse>
{
    public DateTimeOffset? From { get; set; }

    public DateTimeOffset? To { get; set; }

}