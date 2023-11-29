namespace LockerService.Application.Features.Orders.Commands;

public class OrderDetailCommandItemValidator : AbstractValidator<OrderDetailItemCommand>
{
    public OrderDetailCommandItemValidator()
    {
        RuleFor(model => model.ServiceId)
            .NotNull()
            .GreaterThan(0);
        
        RuleFor(model => model.Quantity)
            .NotNull()
            .GreaterThan(0);
    }
}

public class OrderDetailItemCommand
{
    public long ServiceId { get; set; }
    
    public float Quantity { get; set; }
}