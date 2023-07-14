namespace LockerService.Application.Orders.Commands;

public class ConfirmOrderCommand : IRequest<OrderResponse>
{
    public long Id { get; set; }
}