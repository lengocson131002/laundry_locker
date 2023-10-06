using LockerService.Application.Features.Orders.Models;

namespace LockerService.Application.Features.Orders.Commands;

public class CancelOrderCommand : IRequest<OrderResponse>
{
    public long OrderId { get; set; }
}