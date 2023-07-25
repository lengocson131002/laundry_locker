using LockerService.Application.Common.Persistence;
using LockerService.Application.Common.Persistence.Repositories;
using LockerService.Infrastructure.Persistence;

namespace LockerService.Infrastructure.Repositories;

public class UnitOfWork :  BaseUnitOfWork, IUnitOfWork
{
    private IAccountRepository? _accountRepository;
    
    private IAddressRepository? _addressRepository;
    
    private ILocationRepository? _locationRepository;
    
    private IHardwareRepository? _hardwareRepository;
    
    private ILockerRepository? _lockerRepository;

    private ILockerTimelineRepository? _lockerTimelineRepository;

    private IOrderRepository? _orderRepository;

    private IOrderTimelineRepository? _orderTimelineRepository;

    private IServiceRepository? _serviceRepository;

    private IOrderDetailRepository? _orderDetailRepository;

    private IBillRepository? _billRepository;
    
    private IStoreRepository? _storeRepository;

    private IStaffLockerRepository? _staffLockerRepository;

    private IBoxRepository? _boxRepository;

    private ITokenRepository? _tokenRepository;
    
    private readonly ApplicationDbContext _dbContext;

    public UnitOfWork(ApplicationDbContext dbContext) : base(dbContext)
    {
        _dbContext = dbContext;
    }

    public IAccountRepository AccountRepository => _accountRepository ??= new AccountRepository(_dbContext);

    public IAddressRepository AddressRepository => _addressRepository ??= new AddressRepository(_dbContext);

    public IHardwareRepository HardwareRepository => _hardwareRepository ??= new HardwareRepository(_dbContext);

    public ILocationRepository LocationRepository => _locationRepository ??= new LocationRepository(_dbContext);

    public ILockerRepository LockerRepository => _lockerRepository ??= new LockerRepository(_dbContext);


    public ILockerTimelineRepository LockerTimelineRepository =>
        _lockerTimelineRepository ??= new LockerTimelineRepository(_dbContext);
    
    public IOrderRepository OrderRepository => _orderRepository ??= new OrderRepository(_dbContext);

    public IOrderTimelineRepository OrderTimelineRepository =>
        _orderTimelineRepository ??= new OrderTimelineRepository(_dbContext);

    public IServiceRepository ServiceRepository => _serviceRepository ??= new ServiceRepository(_dbContext);

    public IOrderDetailRepository OrderDetailRepository =>
        _orderDetailRepository ??= new OrderDetailRepository(_dbContext);

    public IBillRepository BillRepository => _billRepository ??= new BillRepository(_dbContext);
        
    public IStoreRepository StoreRepository => _storeRepository ??= new StoreRepository(_dbContext);
    
    public IStaffLockerRepository StaffLockerRepository => _staffLockerRepository ??= new StaffLockerRepository(_dbContext);

    public IBoxRepository BoxRepository => _boxRepository ??= new BoxRepository(_dbContext);
 
    public ITokenRepository TokenRepository => _tokenRepository ??= new TokenRepository(_dbContext);
}