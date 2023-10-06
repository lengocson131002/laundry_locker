namespace LockerService.Application.Features.Dashboard.Models;
public class DashboardStoreItem
{
    public long Id { get; set; }

    public string Name { get; set; } = default!;

    public StoreStatus? Status { get; set; }

    public string? Image { get; set; }
    
    
    public int StaffCount { get; set; }
    
    public int LockerCount { get; set; }
    
    public int OrderCount { get; set; }
    
    public int ServiceCount { get; set; }
    
    public decimal Revenue { get; set; }
    
    public DateTimeOffset? CreatedAt { get; set; }
    
    
    public DateTimeOffset? UpdatedAt { get; set; }

}