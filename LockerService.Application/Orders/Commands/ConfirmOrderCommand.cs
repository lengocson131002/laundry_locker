namespace LockerService.Application.Orders.Commands;

public class ConfirmOrderCommand : IRequest<OrderResponse>
{
    public long OrderId { get; set; }
}