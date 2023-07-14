namespace LockerService.Application.Dashboard.Handlers;

public class GetOrderDashboardHandler : IRequestHandler<GetOrderDashboardRequest, OrderDashboardResponse>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetOrderDashboardHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public Task<OrderDashboardResponse> Handle(GetOrderDashboardRequest request,
        CancellationToken cancellationToken)
    {
        // var query = await _unitOfWork.OrderRepository.GetAsync(
        //     predicate: order => OrderStatus.Completed.Equals(order.Status)
        //              && (request.LockerId == null || order.LockerId == request.LockerId)
        //              && (request.From == null || order.CreatedAt >= request.From)
        //              && (request.To == null || order.CreatedAt <= request.To)
        //
        // );
        //
        // var totalOrders = await query.CountAsync(cancellationToken);
        // var totalRevenue = 0;
        //
        // var statistic = query
        //     .GroupBy(o => o.Service)
        //     .Select(item => new ServiceStatisticItem()
        //     {
        //         Service = item.Key.Name,
        //         TotalOrders = item.Count(),
        //         TotalRevenue = Math.Round(item.Sum(order => order.Fee ?? 0), 2, MidpointRounding.AwayFromZero)
        //     })
        //     .ToList();
        //
        // return new OrderDashboardResponse()
        // {
        //     TotalOrders = totalOrders,
        //     TotalRevenue = Math.Round(totalRevenue, 2, MidpointRounding.AwayFromZero),
        //     ServiceStatistic = statistic
        // };
        return null;
    }
}