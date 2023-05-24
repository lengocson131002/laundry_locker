namespace LockerService.Application.Services.Commands;

public class RemoveServiceCommand : IRequest<StatusResponse>
{
    public int LockerId { get; set; }
    
    public int ServiceId { get; set; }
}