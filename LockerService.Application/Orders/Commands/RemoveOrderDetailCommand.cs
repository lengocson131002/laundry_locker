namespace LockerService.Application.Orders.Commands;

public record RemoveOrderDetailCommand(int OrderId, int DetailId) : IRequest<OrderItemResponse>;
