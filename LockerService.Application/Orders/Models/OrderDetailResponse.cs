namespace LockerService.Application.Orders.Models;

public class OrderDetailResponse
{
    public int Id { get; set; }

    public string? PinCode { get; set; }

    public DateTimeOffset? PinCodeIssuedAt { get; set; }

    public int SendBoxOrder { get; set; }

    public int? ReceiveBoxOrder { get; set; }

    public string OrderPhone { get; set; } = default!;

    public string? ReceivePhone { get; set; }

    public DateTimeOffset? ReceiveTime { get; set; }

    public DateTimeOffset? ActualReceiveTime { get; set; }
    
    public double? Amount { get; set; }

    public double? Fee { get; set; }

    public OrderStatus Status { get; set; }
    
    public string? Description { get; set; }

    public string? LockerAddress { get; set; }

    public DateTimeOffset CreatedAt { get; set; }

    public DateTimeOffset UpdatedAt { get; set; }

    public IList<OrderTimelineResponse> Timelines { get; set; } = new List<OrderTimelineResponse>();

    public ServiceResponse Service { get; set; } = default!;

    public LockerResponse Locker { get; set; } = default!;
}