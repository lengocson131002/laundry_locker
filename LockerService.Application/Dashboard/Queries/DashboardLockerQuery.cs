namespace LockerService.Application.Dashboard.Queries;

public class DashboardLockerQuery : IRequest<ListResponse<DashboardLockerItem>>
{
    public long? StoreId { get; set; }
}