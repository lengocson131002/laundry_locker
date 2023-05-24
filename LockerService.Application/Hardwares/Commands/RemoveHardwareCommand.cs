namespace LockerService.Application.Hardwares.Commands;

public class RemoveHardwareCommand : IRequest<StatusResponse>
{
    public int LockerId { get; set; }
    
    public int HardwareId { get; set; }
}