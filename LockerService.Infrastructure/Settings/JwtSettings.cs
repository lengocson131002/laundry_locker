using System.ComponentModel.DataAnnotations;

namespace LockerService.Infrastructure.Settings;

public class JwtSettings
{
    public static readonly string ConfigSection = "Jwt";

    [Required]
    public string Key { get; set; } = default!;

    [Required]
    public string Issuer { get; set; } = default!;

    [Required]
    public string Audience { get; set; } = default!;
    
    [Required]
    [Range(0, Int32.MaxValue)]
    public int TokenExpire { get; set; }
    
    [Required]
    [Range(0, Int32.MaxValue)]
    public int RefreshTokenExpire { get; set; }
}