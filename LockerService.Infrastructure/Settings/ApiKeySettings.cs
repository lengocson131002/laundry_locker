using System.ComponentModel.DataAnnotations;

namespace LockerService.Infrastructure.Settings;

public class ApiKeySettings
{
    public static readonly string ConfigSection = "ApiKey";

    [Required]
    public string Key { get; set; } = default!;
}