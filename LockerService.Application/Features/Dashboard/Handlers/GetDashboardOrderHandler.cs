using LockerService.Application.Common.Persistence.Repositories;
using LockerService.Application.Features.Dashboard.Models;
using LockerService.Application.Features.Dashboard.Queries;

namespace LockerService.Application.Features.Dashboard.Handlers;

public class GetDashboardOrderHandler : IRequestHandler<DashboardOrderQuery, DashboardOrderResponse>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetDashboardOrderHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<DashboardOrderResponse> Handle(DashboardOrderQuery request, CancellationToken cancellationToken)
    {
        var orderQuery = _unitOfWork.OrderRepository
            .GetOrders(request.StoreId, request.LockerId, request.From, request.To);

        // Overview
        var completedOrderCount = await orderQuery
            .Where(order => order.IsCompleted)
            .CountAsync(cancellationToken);

        var revenue = await orderQuery
            .Where(order => order.IsCompleted)
            .SumAsync(order => order.TotalPrice, cancellationToken);
        
        var completedTypes = await orderQuery
            .Where(order => order.IsCompleted)
            .GroupBy(order => order.Type)
            .Select(groupItem => new OrderTypeCount()
            {
                Type = groupItem.Key,
                Count = groupItem.Count(),
                Revenue = groupItem.Sum(order => order.TotalPrice)
            }).ToListAsync(cancellationToken);
        
        foreach (var type in Enum.GetValues(typeof(OrderType)).Cast<OrderType>())
        {
            if (!completedTypes.Any(item => Equals(item.Type, type)))
            {
                completedTypes.Add(new OrderTypeCount()
                {
                    Type = type,
                    Count = 0
                });
            }
        }
        var overview = new OrderOverview()
        {
            Completed = completedOrderCount,
            Revenue = revenue,
            OrderTypes = completedTypes
        };
        
        // Status count
        var orderStatuses = await orderQuery
            .GroupBy(order => order.Status)
            .Select(groupItem => new OrderStatusCount
            {
                Status = groupItem.Key,
                Count = groupItem.Count()
            }).ToListAsync(cancellationToken);
        foreach (var status in Enum.GetValues(typeof(OrderStatus)).Cast<OrderStatus>())
        {
            if (!orderStatuses.Any(item => Equals(item.Status, status)))
                orderStatuses.Add(new OrderStatusCount(status));
        }

        // Type count
        var orderTypes = await orderQuery
        .GroupBy(order => order.Type)
        .Select(groupItem => new OrderTypeCount()
        {
            Type = groupItem.Key,
            Count = groupItem.Count(),
            Revenue = groupItem
                .Where(order => order.IsCompleted)
                .Sum(order => order.TotalPrice)
        }).ToListAsync(cancellationToken);
        
        foreach (var type in Enum.GetValues(typeof(OrderType)).Cast<OrderType>())
        {
            if (!orderTypes.Any(item => Equals(item.Type, type)))
            {
                orderTypes.Add(new OrderTypeCount(type));
            }
        }

        return new DashboardOrderResponse
        {
            Overview = overview,
            OrderStatuses = orderStatuses,
            OrderTypes = orderTypes
        };
    }
}