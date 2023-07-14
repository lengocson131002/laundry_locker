namespace LockerService.Application.Hardwares.Commands;

public class RemoveHardwareCommand : IRequest<StatusResponse>
{
    public long LockerId { get; set; }
    
    public long HardwareId { get; set; }
}