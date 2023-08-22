using System.ComponentModel.DataAnnotations;

namespace LockerService.Infrastructure.Settings;

public class ZaloZnsSettings
{
    public static readonly string ConfigSection = "Zalo:Zns";

    [Required] 
    public string SecretKey { get; set; } = default!;

    [Required]
    public string AppId { get; set; } = default!;

    [Required]
    public string AuthUrl { get; set; } = default!;

    [Required]
    public string ZnsUrl { get; set; } = default!;

    [Required]
    public ZaloZnsTemplates Templates { get; set; } = default!;
}

public class ZaloZnsTemplates
{
    [Required]
    public string Otp { get; set; } = default!;
}
