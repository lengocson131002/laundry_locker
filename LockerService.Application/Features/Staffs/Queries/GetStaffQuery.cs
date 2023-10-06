using LockerService.Application.Features.Staffs.Models;

namespace LockerService.Application.Features.Staffs.Queries;

public class GetStaffQuery : IRequest<StaffDetailResponse>
{
    public long Id { get; init; }
}