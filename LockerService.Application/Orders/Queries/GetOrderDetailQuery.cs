namespace LockerService.Application.Orders.Queries;

public record GetOrderDetailQuery(int OrderId, int DetailId) : IRequest<OrderItemResponse>;