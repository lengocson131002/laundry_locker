namespace LockerService.Application.Orders.Commands;

public class CancelOrderCommand : IRequest<OrderResponse>
{
    public long OrderId { get; set; }
}