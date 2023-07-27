using System.ComponentModel.DataAnnotations;

namespace LockerService.Infrastructure.Settings;

public class RabbitMqSettings
{
    public static readonly string ConfigSection = "RabbitMQ";

    [Required]
    public string Host { get; set; } = default!;
    
    [Required]
    [Range(0, ushort.MaxValue)]
    public ushort Port { get; set; }
    
    public string? Username { get; set; }
    
    public string? Password { get; set; }

    public string EndpointNamePrefix { get; set; } = "locker-service";
}