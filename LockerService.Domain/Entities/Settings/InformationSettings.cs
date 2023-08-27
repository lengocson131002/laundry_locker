namespace LockerService.Domain.Entities.Settings;

public class InformationSettings : ISetting
{
    public string CompanyName { get; set; }
    
    public string? ContactPhone { get; set; }

    public string? ContactEmail { get; set; } = default!;

    public string? Facebook { get; set; } = default!;

    public string? Zalo { get; set; } = default!;
        
    public TimeSpan? OpenedAt { get; set; }
    
    public TimeSpan? ClosedAt { get; set; }

    public InformationSettings()
    {
        CompanyName = "Laundry Locker";
        ContactPhone = string.Empty;
        ContactEmail =  string.Empty;
        Facebook =  string.Empty;
        Zalo =  string.Empty;
        OpenedAt = new TimeSpan(8, 0, 0);
        ClosedAt = new TimeSpan(22, 0, 0);
    }
}