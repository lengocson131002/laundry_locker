using LockerService.Application.Features.Hardwares.Models;

namespace LockerService.Application.Features.Hardwares.Commands;

public class RemoveHardwareCommand : IRequest<HardwareResponse>
{
    public long LockerId { get; set; }
    
    public long HardwareId { get; set; }
}