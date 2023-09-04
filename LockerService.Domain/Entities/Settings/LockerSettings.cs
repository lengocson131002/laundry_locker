namespace LockerService.Domain.Entities.Settings;

public class LockerSettings : ISetting
{
    public int AvailableBoxCountWarning { get; set; }

    public LockerSettings()
    {
        AvailableBoxCountWarning = 3;
    }
}