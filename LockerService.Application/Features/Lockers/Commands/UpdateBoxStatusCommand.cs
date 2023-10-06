namespace LockerService.Application.Features.Lockers.Commands;

public class UpdateBoxStatusCommand : IRequest<StatusResponse>
{
    [JsonIgnore]
    public long LockerId { get; set; }
    
    public int BoxNumber { get; set; }
    
    public bool IsActive { get; set; }
}