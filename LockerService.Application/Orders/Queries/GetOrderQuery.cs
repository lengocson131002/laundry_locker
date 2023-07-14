namespace LockerService.Application.Orders.Queries;

public class GetOrderQuery : IRequest<OrderDetailResponse>
{
    public long Id { get; set; } 
}