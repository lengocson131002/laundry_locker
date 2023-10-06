using LockerService.Application.Features.Orders.Models;

namespace LockerService.Application.Features.Orders.Commands;

public record RemoveLaundryItemCommand(long OrderId, long DetailId, long ItemId) : IRequest<LaundryItemResponse>;
