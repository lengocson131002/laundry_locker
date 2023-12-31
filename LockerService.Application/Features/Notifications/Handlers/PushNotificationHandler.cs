using LockerService.Application.Common.Persistence.Repositories;
using LockerService.Application.Common.Services.Notifications;
using LockerService.Application.Features.Notifications.Commands;
using LockerService.Application.Features.Notifications.Models;

namespace LockerService.Application.Features.Notifications.Handlers;

public class PushNotificationHandler : IRequestHandler<PushNotificationCommand, NotificationModel>
{
    private readonly INotifier _notifier;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    
    public PushNotificationHandler(INotifier notifier, IUnitOfWork unitOfWork, IMapper mapper)
    {
        _notifier = notifier;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<NotificationModel> Handle(PushNotificationCommand request, CancellationToken cancellationToken)
    {
        var account = await _unitOfWork.AccountRepository.GetByIdAsync(request.AccountId);
        
        if (account == null)
        {
            throw new ApiException(ResponseCode.AuthErrorAccountNotFound);
        }
        
        account.Password = "12345678";
        var notification = new Notification(
            type: request.Type,
            entityType: EntityType.Account,
            account: account,
            data: GetNotificationData(request.Type),
            saved: false
        );

        await _notifier.NotifyAsync(notification);

        return _mapper.Map<NotificationModel>(notification);
    }

    private object? GetNotificationData(NotificationType notificationType)
    {
        var store = new Store()
        {
            Id = 1,
            Image =
                "https://upload.wikimedia.org/wikipedia/commons/thumb/b/b9/Marks_%26_Spencer_original_penny_bazaar_%2824th_June_2013%29.jpg/800px-Marks_%26_Spencer_original_penny_bazaar_%2824th_June_2013%29.jpg",
            Name = "Store 1",
            Status = StoreStatus.Active,
            ContactPhone = "0367537978",
            Description = "Cửa hàng 1", 
            CreatedAt = DateTimeOffset.UtcNow,
            UpdatedAt = DateTimeOffset.UtcNow,
        };


        var locker = new Locker()
        {
            Id = 1,
            Image = "https://noithathoaphat123.com/data/Product/tu984-3k_1666769082.jpg",
            Description = "Locker Chung cư Vinhome",
            Name = "Locker Chung cư Vinhome",
            Status = LockerStatus.Active,
            CreatedAt = DateTimeOffset.UtcNow,
            UpdatedAt = DateTimeOffset.UtcNow,
            Code = "LO1690875462",
            IpAddress = "192.168.1.5",
            MacAddress = "cc:2f:71:00:9c:9e",
            Store = store
        };

        var box2 = new Box()
        {
            Id = 1,
            Number = 2,
            CreatedAt = DateTimeOffset.UtcNow,
            UpdatedAt = DateTimeOffset.UtcNow,
            IsActive = true
        };

        var account1 = new Account()
        {
            Id = 1,
            Status = AccountStatus.Active,
            Role = Role.Customer,
            Username = "0367537978",
            FullName = "Lê Ngọc Sơn",
            PhoneNumber = "0367537978",
            Avatar = "https://noithathoaphat123.com/data/Product/tu984-3k_1666769082.jpg",
        };
        
        var account2 = new Account()
        {
            Id = 1,
            Status = AccountStatus.Active,
            Role = Role.Customer,
            Username = "0367537978",
            FullName = "Lê Ngọc Sơn",
            PhoneNumber = "0367537978",
            Avatar = "https://noithathoaphat123.com/data/Product/tu984-3k_1666769082.jpg",
        };

        var order = new Order()
        {
            Id = 26,
            SendBox = box2,
            Sender = account1,
            Receiver = account2,
            Type = OrderType.Laundry,
            Status = OrderStatus.Waiting,
            StoragePrice = 10000,
            Discount = 0,
            ExtraFee = 4000,
            CancelReason = OrderCancelReason.Timeout,
            PinCode = "562486",
            CreatedAt = DateTimeOffset.UtcNow,
            UpdatedAt = DateTimeOffset.UtcNow,
            Locker = locker,
        };
        
        switch (notificationType)
        {
            case NotificationType.AccountOtpCreated:
                return "012345";
            
            case NotificationType.SystemLockerConnected:
            case NotificationType.SystemLockerDisconnected:
            case NotificationType.SystemLockerBoxOverloaded:
            case NotificationType.SystemLockerBoxWarning:
                return locker;
            
            case NotificationType.SystemOrderOverTime:
            case NotificationType.SystemOrderCreated:
            case NotificationType.CustomerOrderCreated:
            case NotificationType.CustomerOrderCanceled:
            case NotificationType.CustomerOrderCompleted:
            case NotificationType.CustomerOrderReturned:
            case NotificationType.CustomerOrderOverTime:
                return order;
        }

        return null;
    }
}