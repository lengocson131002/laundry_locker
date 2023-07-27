namespace LockerService.Application.Orders.Commands;

public record ProcessOrderCommand(long Id) : IRequest<OrderResponse>;