using LockerService.Application.Features.Locations.Models;

namespace LockerService.Application.Features.Orders.Models;

public class OrderDetailResponse : OrderResponse
{
    public IList<OrderTimelineResponse> Timelines { get; set; } = new List<OrderTimelineResponse>();

    public IList<OrderItemResponse> Details { get; set; } = new List<OrderItemResponse>();

    public LocationResponse? DeliveryAddress { get; set; }

}