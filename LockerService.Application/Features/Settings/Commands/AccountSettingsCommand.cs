namespace LockerService.Application.Features.Settings.Commands;

public class AccountSettingsCommandValidator: AbstractValidator<AccountSettingsCommand>
{
    public AccountSettingsCommandValidator()
    {
        RuleFor(model => model.MaxWrongLoginCount)
            .GreaterThanOrEqualTo(3);

        RuleFor(model => model.WrongLoginBlockTimeInMinutes)
            .GreaterThanOrEqualTo(1);
    }
}

public class AccountSettingsCommand
{
    public int MaxWrongLoginCount { get; set; }
    
    public int WrongLoginBlockTimeInMinutes { get; set; }
}