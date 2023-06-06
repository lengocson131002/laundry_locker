namespace LockerService.Application.Orders.Commands;

public class TakeReservationCommandValidator : AbstractValidator<TakeReservationCommand>
{
    public TakeReservationCommandValidator()
    {
        RuleFor(model => model.PinCode)
            .NotNull()
            .Length(6);
    }
}

public class TakeReservationCommand : IRequest<OrderResponse>
{
    public string PinCode { get; set; } = default!;
}