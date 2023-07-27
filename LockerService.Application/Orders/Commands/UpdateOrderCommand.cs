namespace LockerService.Application.Orders.Commands;

public class UpdateOrderValidation : AbstractValidator<UpdateOrderCommand>
{
    public UpdateOrderValidation()
    {
    }
}

public class UpdateOrderCommand : IRequest<OrderResponse>
{
    [JsonIgnore]
    public long Id { get; set; }
    
    [TrimString(true)]
    public string? Description { get; set; }
}