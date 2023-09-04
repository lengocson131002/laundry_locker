using System.ComponentModel.DataAnnotations;

namespace LockerService.Infrastructure.Settings;

public class MqttSettings
{
    public static readonly string ConfigSection = "Mqtt";

    [Required]
    public string Host { get; set; } = default!;
    
    [Required]
    [Range(0, Int32.MaxValue)]
    public int Port { get; set; } = default!;
    
    public string? Username { get; set; }
    
    public string? Password { get; set; }
    
    [Required]
    public string SecretKey { get; set; } = default!;
    
}