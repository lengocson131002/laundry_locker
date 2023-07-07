using LockerService.Application.Locations.Commands;
using LockerService.Application.Services.Commands;
using LockerService.Application.Stores.Commands;

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

        CreateMap<Order, OrderResponse>()
            .ForMember(dest => dest.LockerName,
                opt => opt.MapFrom(src => src.Locker.Name))
            .ForMember(dest => dest.ServiceName,
                opt => opt.MapFrom(src => src.Service.Name));

        CreateMap<Order, OrderDetailResponse>();
        
        // Order timeline
        CreateMap<OrderTimeline, OrderTimelineResponse>();
        
        // Account
        CreateMap<Account, AccountResponse>();
        CreateMap<Account, AccountDetailResponse>();
        
        // Store
        CreateMap<AddStoreCommand, Store>();
        CreateMap<Store, StoreResponse>();
        CreateMap<Store, StoreDetailResponse>();
    }
}