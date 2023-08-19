namespace LockerService.Application.Dashboard.Models;

public class DashboardOrderResponse
{
    public OrderOverview Overview { get; set; } = default!;

    public IList<OrderStatusCount> OrderStatuses { get; set; } = new List<OrderStatusCount>();

    public IList<OrderTypeCount> OrderTypes { get; set; } = new List<OrderTypeCount>();

}

public class OrderStatusCount
{
    public OrderStatus Status { get; set; }

    public int Count { get; set; }

    public OrderStatusCount()
    {
    }

    public OrderStatusCount(OrderStatus status)
    {
        Status = status;
        Count = 0;
    }
}

public class OrderOverview
{
    public long Completed { get; set; }
    
    public decimal Revenue { get; set; }
    
    public IList<OrderTypeCount> OrderTypes { get; set; } = new List<OrderTypeCount>();
    
}

public class OrderTypeCount
{
    public OrderType Type { get; set; }
    
    public int Count { get; set; }
    
    public decimal Revenue { get; set; }

    public OrderTypeCount()
    {
    }

    public OrderTypeCount(OrderType type)
    {
        Type = type;
        Count = 0;
        Revenue = 0;
    }
}