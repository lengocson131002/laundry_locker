namespace LockerService.Application.Dashboard.Models;

public class DashboardOrderResponse
{
    public int CompletedOrderCount { get; set; }
    
    public decimal Revenue { get; set; }

    public IList<OrderStatusCount> OrderStatuses { get; set; } = new List<OrderStatusCount>();
    
}

public class OrderStatusCount
{
    public OrderStatus Status { get; set; }

    public int Count { get; set; }
}