using LockerService.Application.Features.Orders.Models;

namespace LockerService.Application.Features.Orders.Commands;

public class ConfirmOrderCommand : IRequest<OrderResponse>
{
    public long OrderId { get; set; }
}