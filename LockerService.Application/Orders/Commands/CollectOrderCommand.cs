namespace LockerService.Application.Orders.Commands;

public class CollectOrderCommand : IRequest<OrderResponse>
{
    public long OrderId { get; set; }
}