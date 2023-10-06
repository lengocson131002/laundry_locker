using LockerService.Application.Features.Accounts.Models;
using LockerService.Application.Features.Stores.Models;

namespace LockerService.Application.Features.Staffs.Models;

public class StaffResponse : AccountResponse
{
    public string Username { get; set; } = default!;

    public StoreResponse? Store { get; set; }
    
}