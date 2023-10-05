namespace LockerService.Application.Orders.Commands;

public class ReturnOrderCommandValidator : AbstractValidator<ReturnOrderCommand>
{
    public ReturnOrderCommandValidator()
    {
    }
}

public class ReturnOrderCommand : IRequest<OrderResponse>
{
    [JsonIgnore]
    [BindNever]
    public long OrderId { get; set; }
    
}
