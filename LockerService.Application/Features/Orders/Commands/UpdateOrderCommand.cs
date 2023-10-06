using LockerService.Application.Features.Orders.Models;

namespace LockerService.Application.Features.Orders.Commands;

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