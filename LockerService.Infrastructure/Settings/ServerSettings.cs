using System.ComponentModel.DataAnnotations;

namespace LockerService.Infrastructure.Settings;

public class ServerSettings
{
    public const string ConfigSection = "Server";

    [Required]
    public string Host { get; set; } = default!;
}