namespace LockerService.Application.Orders.Commands;

public class ReturnOrderCommandValidator : AbstractValidator<ReturnOrderCommand>
{
    public ReturnOrderCommandValidator()
    {
        RuleFor(model => model.PinCode)
            .NotNull()
            .Length(6);
    }
}

public class ReturnOrderCommand : IRequest<OrderResponse>
{
    public string PinCode { get; set; } = default!;
}
