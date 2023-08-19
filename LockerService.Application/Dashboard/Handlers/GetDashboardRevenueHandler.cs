namespace LockerService.Application.Dashboard.Handlers;

public class GetDashboardRevenueHandler : IRequestHandler<DashboardRevenueQuery, ListResponse<DashboardRevenueItem>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetDashboardRevenueHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ListResponse<DashboardRevenueItem>> Handle(DashboardRevenueQuery request, CancellationToken cancellationToken)
    {
        var orders = _unitOfWork.OrderRepository
            .GetOrders(request.StoreId, request.LockerId)
            .Where(order => order.IsCompleted && order.CreatedAt.Year == request.Year);

        var result = await orders.GroupBy(order => order.CreatedAt.Month)
            .Select(item => new DashboardRevenueItem()
            {
                Month = item.Key,
                OrderCount = item.Count(),
                Revenue = item.Sum(order => order.TotalPrice ?? 0)
            }).ToListAsync(cancellationToken);

        foreach (var month in Enumerable.Range(1, 12))
        {
            if (result.All(item => item.Month != month))
            {
                result.Add(new DashboardRevenueItem(month));
            }
        }
        
        result.Sort((item1, item2) => item1.Month - item2.Month);
        return new ListResponse<DashboardRevenueItem>(result);
    }
}