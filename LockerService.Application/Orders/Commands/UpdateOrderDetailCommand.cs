namespace LockerService.Application.Orders.Commands;

public class UpdateOrderDetailCommandValidator : AbstractValidator<UpdateOrderDetailCommand>
{
    public UpdateOrderDetailCommandValidator()
    {
        RuleFor(model => model.Quantity)
            .GreaterThan(0);
    }
}
public class UpdateOrderDetailCommand : IRequest<OrderItemResponse> {
    
    [JsonIgnore]
    public long OrderId { get; set; }

    [JsonIgnore]
    public long DetailId { get; set; }
    
    public float Quantity { get; set; }
}