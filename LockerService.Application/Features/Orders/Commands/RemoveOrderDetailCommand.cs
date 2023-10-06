using LockerService.Application.Features.Orders.Models;

namespace LockerService.Application.Features.Orders.Commands;

public record RemoveOrderDetailCommand(long OrderId, long DetailId) : IRequest<OrderItemResponse>;
