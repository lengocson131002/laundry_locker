namespace LockerService.Domain.Entities.Settings;

public class InformationSettings : ISetting
{
    public string? CompanyName { get; set; } = default!;
    
    public string? ContactPhone { get; set; } = default!;

    public string? ContactEmail { get; set; } = default!;

    public TimeSpan? OpenAt { get; set; }
    
    public TimeSpan? ClosedAt { get; set; }

    public InformationSettings()
    {
        
    }
}