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
    public int OrderId { get; set; }

    [JsonIgnore]
    public int DetailId { get; set; }
    
    public float Quantity { get; set; }
}