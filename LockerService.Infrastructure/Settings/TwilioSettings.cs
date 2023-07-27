using System.ComponentModel.DataAnnotations;

namespace LockerService.Infrastructure.Settings;

public class TwilioSettings
{
    public static readonly string ConfigSection = "Twilio";

    [Required]
    public string AccountSID { get; set; } = default!;

    [Required]
    public string AuthToken { get; set; } = default!;

    [Required]
    public string PhoneNumber { get; set; } = default!;
}