namespace LockerService.Application.Features.Dashboard.Models;

public class DashboardRevenueItem 
{
    public int Month { get; set; }
    
    public int OrderCount { get; set; }
    
    public decimal Revenue { get; set; }

    public DashboardRevenueItem()
    {
    }

    public DashboardRevenueItem(int month)
    {
        Month = month;
        OrderCount = 0;
        Revenue = 0;
    }
}

