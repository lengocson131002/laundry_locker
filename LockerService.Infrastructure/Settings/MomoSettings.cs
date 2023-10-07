using System.ComponentModel.DataAnnotations;

namespace LockerService.Infrastructure.Settings;

public class MomoSettings
{
    public static readonly string ConfigSection = "Payment:Momo";
    
    [Required]
    public string PartnerCode { get; set; } = default!;
    
    [Required]
    public string AccessKey { get; set; } = default!;

    [Required]
    public string SecretKey { get; set; } = default!;

    [Required]
    public string PaymentEndpoint { get; set; } = default!;

    [Required]
    public string IpnUrl { get; set; } = default!;

    public string RedirectUrl { get; set; } = string.Empty;
}