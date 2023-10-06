using LockerService.Application.Features.Orders.Models;

namespace LockerService.Application.Features.Orders.Commands;

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
