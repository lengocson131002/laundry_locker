using System.ComponentModel.DataAnnotations;

namespace LockerService.Infrastructure.Settings;

public class AwsS3Settings
{
    public static readonly string ConfigSection = "Aws:S3";
    
    [Required]
    public string BucketName { get; set; } = default!;

    [Required] 
    public string Region { get; set; } = default!;

    [Required]
    public string AccessKey { get; set; } = default!;

    [Required]
    public string SecretKey { get; set; } = default!;
}