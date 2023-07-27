using System.ComponentModel.DataAnnotations;

namespace LockerService.Infrastructure.Settings;

public class RedisSettings
{
    public static readonly string ConfigSection = "Redis";
        
    [Required] 
    public string Host { get; set; } = default!;
    
    [Required]
    [Range(0, Int32.MaxValue)]
    public int Port { get; set; }

    public string? Password { get; set; }

}