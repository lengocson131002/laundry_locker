namespace LockerService.Application.Settings.Commands;

public class ZaloAuthSettingCommandValidator : AbstractValidator<ZaloAuthSettingCommand>
{
    public ZaloAuthSettingCommandValidator()
    {
        RuleFor(model => model.AccessToken)
            .NotEmpty();
        
        RuleFor(model => model.RefreshToken)
            .NotEmpty();
    }
}
public class ZaloAuthSettingCommand
{
    public string AccessToken { get; set; } = default!;

    public string RefreshToken { get; set; } = default!;
}