namespace LockerService.Application.Hardwares.Queries;

public class GetHardwareQuery : IRequest<HardwareDetailResponse>
{
    public long LockerId { get; set; }
    
    public long HardwareId { get; set; }
}