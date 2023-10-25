using LockerService.Application.Features.Orders.Models;

namespace LockerService.Application.Features.Orders.Commands;

public record AddMoreItemsCommand(long OrderId) : IRequest<OrderResponse>;
