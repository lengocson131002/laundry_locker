namespace LockerService.Application.Orders.Commands;

public class CheckoutOrderCommandValidation : AbstractValidator<CheckoutOrderCommand>
{
    public CheckoutOrderCommandValidation()
    {
        RuleFor(model => model.PinCode)
            .NotNull()
            .Length(6);
    }
}

public class CheckoutOrderCommand : IRequest<OrderResponse>
{
    public string PinCode { get; set; } = default!;
}