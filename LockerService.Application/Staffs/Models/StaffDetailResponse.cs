namespace LockerService.Application.Staffs.Models;

public class StaffDetailResponse : AccountDetailResponse
{
    public StoreDetailResponse Store { get; set; } = default!;
}