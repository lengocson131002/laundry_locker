namespace LockerService.Application.Common.Persistence.Repositories;

public interface IOrderDetailRepository : IBaseRepository<OrderDetail>
{
    Task<IList<OrderDetail>> GetOrderDetails(long orderId);
}