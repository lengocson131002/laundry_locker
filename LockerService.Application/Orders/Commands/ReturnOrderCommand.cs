namespace LockerService.Application.Orders.Commands;

public record ReturnOrderCommand(long Id) : IRequest<OrderResponse>;