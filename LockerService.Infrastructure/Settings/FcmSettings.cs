using System.ComponentModel.DataAnnotations;

namespace LockerService.Infrastructure.Settings;

public class FcmSettings
{
    public static string ConfigSection = "Fcm";
    
    [Required]
    public string ProjectId { get; set; } = default!;

    [Required]
    public string PrivateKey { get; set; } = default!;
    
    [Required]
    public string ClientEmail { get; set; } = default!;
    
    [Required]
    public string TokenUri { get; set; } = default!;
}