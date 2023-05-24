namespace LockerService.Application.Hardwares.Models;

public class HardwareDetailResponse : HardwareResponse
{
    public DateTimeOffset CreatedAt { get; set; }
    
    public long? CreatedBy { get; set; }
    
    public DateTimeOffset UpdatedAt { get; set; }
    
    public long? UpdatedBy { get; set; }
    
    public DateTimeOffset? DeletedAt { get; set; }
    
    public long? DeletedBy { get; set; }
    
}