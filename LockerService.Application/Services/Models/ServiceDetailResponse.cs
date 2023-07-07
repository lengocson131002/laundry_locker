namespace LockerService.Application.Services.Models;

public class ServiceDetailResponse : ServiceResponse
{
    public DateTimeOffset? DeletedAt { get; set; }
    
    public long? DeletedBy { get; set; }
}