using LockerService.Application.Features.Orders.Models;

namespace LockerService.Application.Features.Orders.Queries;

public record GetOrderDetailsQuery(long OrderId) : IRequest<ListResponse<OrderItemResponse>>;