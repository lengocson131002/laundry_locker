namespace LockerService.Application.Orders.Queries;

public class GetOrderQuery : IRequest<OrderDetailResponse>
{
    public int Id { get; set; } 
}