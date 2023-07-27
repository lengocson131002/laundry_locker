namespace LockerService.Application.Staffs.Models;

public class StaffResponse : AccountResponse
{
    public StoreResponse? Store { get; set; }
}