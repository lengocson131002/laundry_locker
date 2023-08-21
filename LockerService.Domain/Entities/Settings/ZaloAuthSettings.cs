namespace LockerService.Domain.Entities.Settings;

public class ZaloAuthSettings : ISetting
{
    public string AccessToken { get; set; } = default!;

    public string RefreshToken { get; set; } = default!;
};