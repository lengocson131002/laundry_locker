namespace LockerService.Application.Orders.Commands;

public record RemoveLaundryItemCommand(long OrderId, long DetailId, long ItemId) : IRequest<LaundryItemResponse>;
