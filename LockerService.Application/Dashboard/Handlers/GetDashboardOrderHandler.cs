namespace LockerService.Application.Dashboard.Handlers;

public class GetDashboardOrderHandler : IRequestHandler<DashboardOrderQuery, DashboardOrderResponse>
{

    private readonly IUnitOfWork _unitOfWork;

    public GetDashboardOrderHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<DashboardOrderResponse> Handle(DashboardOrderQuery request, CancellationToken cancellationToken)
    {
        var completedOrderCount = await _unitOfWork.OrderRepository
            .GetCompletedOrders(request.From, request.To)
            .CountAsync(cancellationToken);

        var revenue = await _unitOfWork.OrderRepository
            .GetCompletedOrders(request.From, request.To)
            .SumAsync(order => order.TotalPrice ?? 0, cancellationToken);

        var orderStatuses = await _unitOfWork.OrderRepository
            .Get(order => (request.From == null || order.CreatedAt >= request.From) 
                          && (request.To == null || order.CreatedAt <= request.To))
            .GroupBy(order => order.Status)
            .Select(groupItem => new OrderStatusCount()
            {
                Status = groupItem.Key,
                Count = groupItem.Count()
            }).ToListAsync(cancellationToken);
        
        foreach (var status in  Enum.GetValues(typeof(OrderStatus)).Cast<OrderStatus>())
        {
            if (!orderStatuses.Any(item => Equals(item.Status, status)))
            {
                orderStatuses.Add(new OrderStatusCount()
                {
                    Status = status,
                    Count = 0
                });
            }
        }
        
        return new DashboardOrderResponse()
        {
            CompletedOrderCount = completedOrderCount,
            Revenue = revenue,
            OrderStatuses = orderStatuses
        };
    }
}