namespace LockerService.Application.Dashboard.Models;

public class DashboardLockerLocationItem
{
    public long Id { get; set; }

    public string Name { get; set; } = default!;

    public string Code { get; set; } = default!;
    
    public string? Image { get; set; }
    
    public LockerStatus Status { get; set; }
    
    public LocationResponse Location { get; set; } = default!;
    
    public DateTimeOffset? CreatedAt { get; set; }
}