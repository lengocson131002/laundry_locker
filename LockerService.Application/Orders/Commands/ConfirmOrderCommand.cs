namespace LockerService.Application.Orders.Commands;

public class ConfirmOrderCommand : IRequest<OrderResponse>
{
    public int Id { get; set; }
}