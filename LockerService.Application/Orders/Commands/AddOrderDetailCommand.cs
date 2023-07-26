namespace LockerService.Application.Orders.Commands;

public class AddOrderDetailCommandValidator : AbstractValidator<AddOrderDetailCommand>
{
    public AddOrderDetailCommandValidator()
    {
        RuleFor(model => model.ServiceId)
            .NotNull();
        RuleFor(model => model.Quantity)
            .NotNull()
            .GreaterThan(0);
    }
}
public class AddOrderDetailCommand : IRequest<OrderItemResponse>
{
    [JsonIgnore]
    public long OrderId { get; set; }

    public long ServiceId { get; set; }
    
    public float Quantity { get; set; }
}