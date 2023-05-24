namespace LockerService.Application.Orders.Models;

public class OrderResponse
{
    public int Id { get; set; }

    public string OrderPhone { get; set; } = default!;

    public string ReceivePhone { get; set; } = default!;

    public int? SendBoxOrder { get; set; }

    public int? ReceiveBoxOrder { get; set; }

    public OrderStatus Status { get; set; }

    public DateTimeOffset? CreatedAt { get; set; }

    public DateTimeOffset? UpdatedAt { get; set; }
    
    public double? Amount { get; set; }

    public double? Fee { get; set; }

    public int LockerId { get; set; }

    public string LockerName { get; set; } = default!;

    public int ServiceId { get; set; }

    public string ServiceName { get; set; } = default!;
}