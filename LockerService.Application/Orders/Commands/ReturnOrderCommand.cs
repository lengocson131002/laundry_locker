namespace LockerService.Application.Orders.Commands;

public class ReturnOrderCommand : IRequest<OrderResponse>
{
    public int Id { get; set; }
}