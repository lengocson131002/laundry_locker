namespace LockerService.Application.Settings.Commands;

public class OrderSettingsCommandValidator : AbstractValidator<OrderSettingsCommand>
{
    public OrderSettingsCommandValidator()
    {
        RuleFor(model => model.InitTimeoutInMinutes)
            .GreaterThan(0);
        RuleFor(model => model.ReservationInitTimeoutInMinutes)
            .GreaterThan(0);
        RuleFor(model => model.StoragePrice)
            .GreaterThan(0);
        RuleFor(model => model.MaxTimeInHours)
            .GreaterThan(0);
        RuleFor(model => model.ExtraFee)
            .GreaterThan(0);
        RuleFor(model => model.MaxActiveOrderCount)
            .GreaterThan(0);
    }
}

public class OrderSettingsCommand
{
    public int InitTimeoutInMinutes { get; set; }

    public int ReservationInitTimeoutInMinutes { get; set; }

    public decimal StoragePrice { get; set; }

    public float MaxTimeInHours { get; set; }

    public decimal ExtraFee { get; set; }

    public int MaxActiveOrderCount { get; set; }
}