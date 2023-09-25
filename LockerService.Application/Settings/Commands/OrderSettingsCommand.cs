namespace LockerService.Application.Settings.Commands;

public class OrderSettingsCommandValidator : AbstractValidator<OrderSettingsCommand>
{
    public OrderSettingsCommandValidator()
    {
        RuleFor(model => model.InitTimeoutInMinutes)
            .GreaterThanOrEqualTo(1);
        RuleFor(model => model.ReservationInitTimeoutInMinutes)
            .GreaterThanOrEqualTo(1);
        RuleFor(model => model.StoragePrice)
            .GreaterThanOrEqualTo(1);
        RuleFor(model => model.MaxTimeInHours)
            .GreaterThanOrEqualTo(1);
        RuleFor(model => model.ExtraFee)
            .GreaterThanOrEqualTo(1);
        RuleFor(model => model.MaxActiveOrderCount)
            .GreaterThanOrEqualTo(1);
        RuleFor(model => model.ReservationFee)
            .GreaterThanOrEqualTo(1);
        RuleFor(model => model.MinTimeProcessLaundryOrderInHours)
            .GreaterThanOrEqualTo(1);
    }
}

public class OrderSettingsCommand
{
    public int InitTimeoutInMinutes { get; set; }

    public int ReservationInitTimeoutInMinutes { get; set; }

    public decimal StoragePrice { get; set; }

    public int MaxTimeInHours { get; set; }

    public decimal ExtraFee { get; set; }

    public int MaxActiveOrderCount { get; set; }
    
    public decimal ReservationFee { get; set; }
    
    public int MinTimeProcessLaundryOrderInHours { get; set; }
}