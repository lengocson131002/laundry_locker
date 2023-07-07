namespace LockerService.Application.Stores.Models;

public class StoreResponse
{
    public int Id { get; set; }
    public string Name { get; set; } = default!;
    public string? ContactPhone { get; set; }
    public StoreStatus Status { get; set; }
    public LocationResponse Location { get; set; } = default!;
    public string? Image { get; set; }
}