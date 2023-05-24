namespace LockerService.Application.Dashboard.Models;

public class OrderDashboardResponse
{
    public int TotalOrders { get; set; }
    
    public double TotalRevenue { get; set; }

    public IList<ServiceStatisticItem> ServiceStatistic { get; set; } = new List<ServiceStatisticItem>();
}

public class ServiceStatisticItem
{

    public string Service { get; set; } = default!;

    public int TotalOrders { get; set; }

    public double TotalRevenue { get; set; }

}