namespace LockerService.Infrastructure.Settings;

public class QuartzSettings
{
    public static readonly string ConfigSection = "Quartz";

    public string ConnectionString { get; set; } = default!;
}