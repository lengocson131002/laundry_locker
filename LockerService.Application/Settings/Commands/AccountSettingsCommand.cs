namespace LockerService.Application.Settings.Commands;

public class AccountSettingsCommandValidator: AbstractValidator<AccountSettingsCommand>
{
    public AccountSettingsCommandValidator()
    {
        RuleFor(model => model.MaxWrongLoginCount)
            .GreaterThan(0);

        RuleFor(model => model.WrongLoginBlockTimeInMinutes)
            .GreaterThan(0);
    }
}

public class AccountSettingsCommand
{
    public int MaxWrongLoginCount { get; set; }
    
    public int WrongLoginBlockTimeInMinutes { get; set; }
}