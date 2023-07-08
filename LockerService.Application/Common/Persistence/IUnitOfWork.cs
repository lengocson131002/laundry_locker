using LockerService.Application.Common.Persistence.Repositories;

namespace LockerService.Application.Common.Persistence;

public interface IUnitOfWork : IBaseUnitOfWork
{
    IAccountRepository AccountRepository { get; }
    IAccountLockerRepository AccountLockerRepository { get; }
    
    IAddressRepository AddressRepository { get; }

    IHardwareRepository HardwareRepository { get; }

    ILocationRepository LocationRepository { get; }

    ILockerRepository LockerRepository { get; }

    ILockerTimelineRepository LockerTimelineRepository { get; }

    IOrderRepository OrderRepository { get; }
    
    IOrderTimelineRepository OrderTimelineRepository { get; }

    IServiceRepository ServiceRepository { get; }
    IStoreRepository StoreRepository { get; }

}