using LockerService.Application.Common.Persistence.Repositories;
using LockerService.Infrastructure.Persistence.Contexts;
using Microsoft.Extensions.DependencyInjection;

namespace LockerService.Infrastructure.Persistence.Repositories;

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

    private IStoreRepository? _storeRepository;

    private IStaffLockerRepository? _staffLockerRepository;

    private IBoxRepository? _boxRepository;

    private ITokenRepository? _tokenRepository;

    private ISettingRepository? _settingRepository;

    private INotificationRepository? _notificationRepository;
    
    private ILaundryItemRepository? _laundryItemRepository;

    private IAuditRepository? _auditRepository;
    
    private IPaymentRepository? _paymentRepository;

    private IStoreServiceRepository? _storeServiceRepository;
    
    private readonly ApplicationDbContext _dbContext;

    private readonly IServiceScopeFactory _scopeFactory;
    public UnitOfWork(ApplicationDbContext dbContext, IServiceScopeFactory scopeFactory) : base(dbContext)
    {
        _dbContext = dbContext;
        _scopeFactory = scopeFactory;
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

    public IStoreRepository StoreRepository => _storeRepository ??= new StoreRepository(_dbContext);
    
    public IStaffLockerRepository StaffLockerRepository => _staffLockerRepository ??= new StaffLockerRepository(_dbContext);

    public IBoxRepository BoxRepository => _boxRepository ??= new BoxRepository(_dbContext);
 
    public ITokenRepository TokenRepository => _tokenRepository ??= new TokenRepository(_dbContext);

    public ISettingRepository SettingRepository => _settingRepository ??= new SettingRepository(_dbContext, _scopeFactory);

    public INotificationRepository NotificationRepository =>
        _notificationRepository ??= new NotificationRepository(_dbContext);

    public ILaundryItemRepository LaundryItemRepository => _laundryItemRepository ??= new LaundryItemRepository(_dbContext);

    public IAuditRepository AuditRepository => _auditRepository ??= new AuditRepository(_dbContext);

    public IPaymentRepository PaymentRepository => _paymentRepository ??= new PaymentRepository(_dbContext);

    public IStoreServiceRepository StoreServiceRepository =>
        _storeServiceRepository ??= new StoreLockerRepository(_dbContext);
}