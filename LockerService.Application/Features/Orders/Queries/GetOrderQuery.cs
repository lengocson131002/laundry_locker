using LockerService.Application.Features.Orders.Models;

namespace LockerService.Application.Features.Orders.Queries;

public class GetOrderQuery : IRequest<OrderDetailResponse>
{
    public long Id { get; set; } 
}