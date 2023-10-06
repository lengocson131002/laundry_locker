namespace LockerService.Application.Features.Dashboard.Models;

public class DashboardLockerItem
{
    public LockerStatus Status { get; set; }
    
    public int Count { get; set; }

    public DashboardLockerItem()
    {
    }

    public DashboardLockerItem(LockerStatus status)
    {
        Status = status;
        Count = 0;
    }
}