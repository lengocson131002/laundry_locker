namespace LockerService.Application.Orders.Commands;

public record CheckoutOrderCommand(long Id) : IRequest<OrderResponse>;