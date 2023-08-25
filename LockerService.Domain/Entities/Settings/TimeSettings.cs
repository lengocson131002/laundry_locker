namespace LockerService.Domain.Entities.Settings;

public class TimeSettings : ISetting
{
    public string TimeZone { get; set; }

    public TimeSettings()
    {
        TimeZone = "Asia/Ho_Chi_Minh";
    }
}