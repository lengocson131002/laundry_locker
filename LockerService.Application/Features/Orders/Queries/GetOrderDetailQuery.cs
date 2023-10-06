using LockerService.Application.Features.Orders.Models;

namespace LockerService.Application.Features.Orders.Queries;

public record GetOrderDetailQuery(long OrderId, long DetailId) : IRequest<OrderItemResponse>;