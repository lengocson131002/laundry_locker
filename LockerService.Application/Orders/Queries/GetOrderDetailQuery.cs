namespace LockerService.Application.Orders.Queries;

public record GetOrderDetailQuery(long OrderId, long DetailId) : IRequest<OrderItemResponse>;