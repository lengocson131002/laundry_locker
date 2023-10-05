namespace LockerService.Application.Orders.Commands;

public class ProcessOrderCommand : IRequest<OrderResponse>
{
    public long OrderId { get; set; }
}