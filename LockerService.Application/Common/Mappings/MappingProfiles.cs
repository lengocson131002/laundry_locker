using LockerService.Application.Locations.Commands;
using LockerService.Application.Services.Commands;

namespace LockerService.Application.Common.Mappings;

public class MappingProfiles : Profile
{
    public MappingProfiles()
    {
        // Locker
        CreateMap<AddLockerCommand, Locker>();
        CreateMap<Locker, LockerResponse>();
        CreateMap<Locker, LockerDetailResponse>();
        CreateMap<LockerTimeline, LockerTimelineResponse>();
        
        // Service
        CreateMap<AddServiceCommand, Service>();
        CreateMap<Service, ServiceResponse>();
        CreateMap<Service, ServiceDetailResponse>();
        
        // Hardware
        CreateMap<AddHardwareCommand, Hardware>();
        CreateMap<Hardware, HardwareResponse>();
        CreateMap<Hardware, HardwareDetailResponse>();
        
        // Location
        CreateMap<LocationCommand, Location>();
        CreateMap<Address, AddressResponse>();
        CreateMap<Location, LocationResponse>();
        
        // Order
        CreateMap<CreateOrderCommand, Order>();
        CreateMap<Order, OrderResponse>();
        
        // Bill
        
        // OrderDetail
        CreateMap<OrderDetail, OrderItemResponse>();

        CreateMap<Order, OrderDetailResponse>();
        
        // Order timeline
        CreateMap<OrderTimeline, OrderTimelineResponse>();
    }
}