namespace LockerService.Application.Features.Settings.Commands;


public class PaymentSettingCommandValidator : AbstractValidator<PaymentSettingsCommand>
{
    public PaymentSettingCommandValidator()
    {
        RuleFor(model => model.MinDeposit)
            .GreaterThanOrEqualTo(1000);

        RuleFor(model => model.PaymentTimeoutInMinutes)
            .GreaterThan(0);
    }
}

public class PaymentSettingsCommand
{
    // Timeout for an payment. Payment will be failed if not completed after this period of time
    public int PaymentTimeoutInMinutes { get; set; }

    // Minimum allowed deposit amount of money
    public decimal MinDeposit { get; set; }
}