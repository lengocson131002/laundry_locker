namespace LockerService.Application.Dashboard.Models;
public class DashboardStoreItem
{
    public long Id { get; set; }

    public string Name { get; set; } = default!;

    public StoreStatus? Status { get; set; }

    public string? Image { get; set; }
    
    public DateTimeOffset? CreatedAt { get; set; }
    
    public int StaffCount { get; set; }
    
    public int LockerCount { get; set; }
    
    public int OrderCount { get; set; }
    
    public decimal Revenue { get; set; }
}