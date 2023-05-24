namespace LockerService.Application.Hardwares.Queries;

public class GetHardwareQuery : IRequest<HardwareDetailResponse>
{
    public int LockerId { get; set; }
    
    public int HardwareId { get; set; }
}