using LockerService.Application.Features.Orders.Models;

namespace LockerService.Application.Features.Orders.Commands;

public class ProcessOvertimeOrderCommand : IRequest<OrderDetailResponse>
{
    public long OrderId { get; set; }
}