namespace LockerService.Application.Dashboard.Models;

public class DashboardOverviewResponse
{
    public int StoreCount { get; set; }
    
    public int LockerCount { get; set; }
    
    public int StaffCount { get; set; }
    
    public int CustomerCount { get; set; }
    
    public int ServiceCount { get; set; }
}