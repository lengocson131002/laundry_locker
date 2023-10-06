using LockerService.Application.Features.Hardwares.Models;

namespace LockerService.Application.Features.Hardwares.Queries;

public class GetHardwareQuery : IRequest<HardwareDetailResponse>
{
    public long LockerId { get; set; }
    
    public long HardwareId { get; set; }
}