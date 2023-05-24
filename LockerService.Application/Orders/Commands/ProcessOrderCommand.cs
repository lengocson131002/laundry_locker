namespace LockerService.Application.Orders.Commands;

public class ProcessOrderCommand : IRequest<OrderResponse>
{
    public int Id { get; set; }
}