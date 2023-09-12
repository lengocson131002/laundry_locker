namespace LockerService.Application.Staffs.Models;

public class StaffResponse : AccountResponse
{
    public string Username { get; set; } = default!;

    public StoreResponse? Store { get; set; }
    
}