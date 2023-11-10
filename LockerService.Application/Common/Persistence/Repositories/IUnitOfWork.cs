namespace LockerService.Application.Common.Persistence.Repositories;

public interface IUnitOfWork : IBaseUnitOfWork
{
    IAccountRepository AccountRepository { get; }

    IAddressRepository AddressRepository { get; }

    IHardwareRepository HardwareRepository { get; }

    ILocationRepository LocationRepository { get; }

    ILockerRepository LockerRepository { get; }

    ILockerTimelineRepository LockerTimelineRepository { get; }

    IOrderRepository OrderRepository { get; }

    IOrderTimelineRepository OrderTimelineRepository { get; }

    IServiceRepository ServiceRepository { get; }

    IOrderDetailRepository OrderDetailRepository { get; }

    IStoreRepository StoreRepository { get; }

    IStaffLockerRepository StaffLockerRepository { get; }

    ITokenRepository TokenRepository { get; }

    IBoxRepository BoxRepository { get; }

    ISettingRepository SettingRepository { get; }

    INotificationRepository NotificationRepository { get; }

    ILaundryItemRepository LaundryItemRepository { get; }

    IAuditRepository AuditRepository { get; }

    IPaymentRepository PaymentRepository { get; }

    IStoreServiceRepository StoreServiceRepository { get; }

    IShippingPriceRepository ShippingPriceRepository { get; }

}