namespace LockerService.Application.Settings.Commands;

public class LockerSettingCommandValidator : AbstractValidator<LockerSettingsCommand>
{
    public LockerSettingCommandValidator()
    {
        RuleFor(model => model.AvailableBoxCountWarning)
            .GreaterThanOrEqualTo(1);
    }
}

public class LockerSettingsCommand 
{
    public int AvailableBoxCountWarning { get; set; }
}