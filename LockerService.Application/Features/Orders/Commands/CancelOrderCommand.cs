using LockerService.Application.Features.Orders.Models;

namespace LockerService.Application.Features.Orders.Commands;

public record CancelOrderCommand(long OrderId) : IRequest<OrderResponse>;
