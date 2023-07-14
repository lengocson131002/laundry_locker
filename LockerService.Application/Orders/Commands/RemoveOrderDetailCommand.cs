namespace LockerService.Application.Orders.Commands;

public record RemoveOrderDetailCommand(long OrderId, long DetailId) : IRequest<OrderItemResponse>;
