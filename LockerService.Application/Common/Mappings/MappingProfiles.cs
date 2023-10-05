using LockerService.Application.Audits.Models;
using LockerService.Application.Notifications.Models;
using LockerService.Application.Payments.Models;
using LockerService.Application.Services.Commands;
using LockerService.Application.Settings.Commands;
using LockerService.Domain;
using LockerService.Domain.Entities.Settings;

namespace LockerService.Application.Common.Mappings;

public class MappingProfiles : Profile
{
    public MappingProfiles()
    {
        // Locker
        CreateMap<AddLockerCommand, Locker>()
            .ForMember(dest => dest.OrderTypes, options 
                => options.MapFrom(src => src.OrderTypes.Select(type => new LockerOrderType(type))));
        
        CreateMap<Locker, LockerResponse>()
            .ForMember(dest => dest.OrderTypes,option 
                    => option.MapFrom(src => src.OrderTypes.Select(item => item.OrderType)));
        
        CreateMap<Locker, LockerDetailResponse>()
            .ForMember(dest => dest.OrderTypes,option 
                => option.MapFrom(src => src.OrderTypes.Select(item => item.OrderType)));;
        
        CreateMap<LockerTimeline, LockerTimelineResponse>();
        CreateMap<Box, BoxResponse>();
        CreateMap<Locker, DashboardLockerLocationItem>();
        
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
        CreateMap<InitializeOrderCommand, Order>();
        CreateMap<Order, OrderResponse>();
        CreateMap<Order, BoxOrderResponse>();
        
        // OrderDetail
        CreateMap<OrderDetail, OrderItemResponse>();
        CreateMap<Order, OrderDetailResponse>();
        
        // Order timeline
        CreateMap<OrderTimeline, OrderTimelineResponse>();
        
        // Order laundry items 
        CreateMap<LaundryItem, LaundryItemResponse>();
        CreateMap<AddLaundryItemCommand, LaundryItem>();
        
        // Account
        CreateMap<Account, AccountResponse>();
        CreateMap<Account, AccountDetailResponse>();
        CreateMap<Account, StaffResponse>();
        CreateMap<Account, StaffDetailResponse>();
        CreateMap<Account, CustomerResponse>();
        CreateMap<Account, CustomerDetailResponse>();
        
        // Store
        CreateMap<AddStoreCommand, Store>();
        CreateMap<Store, StoreResponse>();
        CreateMap<Store, StoreDetailResponse>();
        CreateMap<Store, DashboardStoreItem>();
        
        // Settings
        CreateMap<InformationSettingsCommand, InformationSettings>();
        CreateMap<AccountSettingsCommand, AccountSettings>();
        CreateMap<OrderSettingsCommand, OrderSettings>();
        CreateMap<ZaloAuthSettingsCommand, ZaloAuthSettings>();
        CreateMap<TimeSettingsCommand, TimeSettings>();
        CreateMap<LockerSettingsCommand, LockerSettings>();
        
        // Notifications
        CreateMap<Notification, NotificationModel>();
        
        // Audit
        CreateMap<Audit, AuditResponse>();
        
        // Payments
        CreateMap<Payment, PaymentResponse>();

    }

}