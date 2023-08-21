using LockerService.Application.Bills.Models;
using LockerService.Application.Customers.Models;
using LockerService.Application.Locations.Commands;
using LockerService.Application.Notifications.Models;
using LockerService.Application.Services.Commands;
using LockerService.Application.Settings.Commands;
using LockerService.Application.Staffs.Models;
using LockerService.Domain.Entities.Settings;

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
        CreateMap<CreateOrderCommand, Order>();
        CreateMap<Order, OrderResponse>();
        CreateMap<Order, BoxOrderResponse>();
        
        // Bill
        CreateMap<Bill, BillResponse>();
        
        // OrderDetail
        CreateMap<OrderDetail, OrderItemResponse>();
        CreateMap<Order, OrderDetailResponse>();
        
        // Order timeline
        CreateMap<OrderTimeline, OrderTimelineResponse>();
        
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
        CreateMap<ZaloAuthSettingCommand, ZaloAuthSettings>();
        
        // Notifications
        CreateMap<Notification, NotificationModel>();
    }

}