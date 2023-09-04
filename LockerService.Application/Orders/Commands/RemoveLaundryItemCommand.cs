namespace LockerService.Application.Orders.Commands;

public record RemoveLaundryItemCommand(long OrderId, long ItemId) : IRequest<LaundryItemResponse>;
