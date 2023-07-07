namespace LockerService.Application.Orders.Commands;

public class ProcessOrderCommandValidator : AbstractValidator<ProcessOrderCommand>
{
    public ProcessOrderCommandValidator()
    {
        RuleFor(model => model.PinCode)
            .NotNull()
            .MinimumLength(6);
    }    
}

public class ProcessOrderCommand : IRequest<OrderResponse>
{
    public string PinCode { get; set; } = default!;
}