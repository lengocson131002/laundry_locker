namespace LockerService.Application.Accounts.Models;

public class StoreResponse
{
    public int Id { get; set; }
    public string Name { get; set; } 
    public string? ContactPhone { get; set; }
    public StoreStatus Status { get; set; }
    public int LocationId { get; set; }
    public string? Image { get; set; }
}