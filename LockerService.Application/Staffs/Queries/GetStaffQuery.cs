using LockerService.Application.Staffs.Models;

namespace LockerService.Application.Staffs.Queries;

public class GetStaffQuery : IRequest<StaffDetailResponse>
{
    public long Id { get; init; }
}