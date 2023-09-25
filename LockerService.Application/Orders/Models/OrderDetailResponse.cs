using LockerService.Application.Bills.Models;

namespace LockerService.Application.Orders.Models;

public class OrderDetailResponse : OrderResponse
{
    public IList<OrderTimelineResponse> Timelines { get; set; } = new List<OrderTimelineResponse>();

    public IList<OrderItemResponse> Details { get; set; } = new List<OrderItemResponse>();

    public BillResponse? Bill { get; set; }
    
    public LocationResponse? DeliveryAddress { get; set; }
    
    public bool UpdatedInfo { get; set; }

}