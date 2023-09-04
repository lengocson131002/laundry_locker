namespace LockerService.Application.Settings.Commands;

public class ZaloAuthSettingCommandValidator : AbstractValidator<ZaloAuthSettingsCommand>
{
    public ZaloAuthSettingCommandValidator()
    {
        RuleFor(model => model.AccessToken)
            .NotEmpty();
        
        RuleFor(model => model.RefreshToken)
            .NotEmpty();
    }
}
public class ZaloAuthSettingsCommand
{
    [TrimString(true)]
    public string AccessToken { get; set; } = default!;

    [TrimString(true)]
    public string RefreshToken { get; set; } = default!;
}