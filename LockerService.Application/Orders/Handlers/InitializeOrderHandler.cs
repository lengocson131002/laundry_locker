using LockerService.Application.EventBus.RabbitMq;
using LockerService.Application.EventBus.RabbitMq.Events.Lockers;
using LockerService.Application.EventBus.RabbitMq.Events.Orders;
using LockerService.Domain.Entities.Settings;

namespace LockerService.Application.Orders.Handlers;

public class InitializeOrderHandler : IRequestHandler<InitializeOrderCommand, OrderResponse>
{
    private readonly ILogger<InitializeOrderHandler> _logger;
    private readonly IMapper _mapper;
    private readonly IRabbitMqBus _rabbitMqBus;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ISettingService _settingService;

    public InitializeOrderHandler(
        ILogger<InitializeOrderHandler> logger,
        IMapper mapper,
        IUnitOfWork unitOfWork,
        IRabbitMqBus rabbitMqBus, 
        ISettingService settingService)
    {
        _logger = logger;
        _mapper = mapper;
        _unitOfWork = unitOfWork;
        _rabbitMqBus = rabbitMqBus;
        _settingService = settingService;
    }

    public async Task<OrderResponse> Handle(InitializeOrderCommand command, CancellationToken cancellationToken)
    {
        var locker = await _unitOfWork.LockerRepository.GetByIdAsync(command.LockerId);
        if (locker == null)
        {
            throw new ApiException(ResponseCode.LockerErrorNotFound);
        }

        // Check Locker status
        if (!LockerStatus.Active.Equals(locker.Status))
        {
            throw new ApiException(ResponseCode.LockerErrorNotActive);
        }

        // Check available boxes
        var availableBox = await _unitOfWork.BoxRepository.FindAvailableBox(locker.Id);
        if (availableBox == null)
        {
            var exception = new ApiException(ResponseCode.LockerErrorNoAvailableBox);
            await _rabbitMqBus.PublishAsync(new LockerOverloadedEvent
            {
                LockerId = locker.Id,
                Time = DateTimeOffset.UtcNow,
                ErrorCode = exception.ErrorCode,
                Error = exception.ErrorMessage
            }, cancellationToken);

            throw exception;
        }

        // Check services
        var details = new List<OrderDetail>();
        if (Equals(command.Type, OrderType.Laundry))
        {
            foreach (var serviceId in command.ServiceIds)
            {
                var service = await _unitOfWork.ServiceRepository.GetStoreService(locker.StoreId, serviceId);
                if (service == null || !service.IsActive)
                    throw new ApiException(ResponseCode.OrderErrorServiceIsNotAvailable);

                var orderDetail = new OrderDetail
                {
                    Service = service,
                    Price = service.Price
                };

                details.Add(orderDetail);
            }
            await _unitOfWork.OrderDetailRepository.AddRange(details);
        }

        // Check sender and receiver
        var senderPhone = command.SenderPhone;
        var sender = await _unitOfWork.AccountRepository.GetCustomerByPhoneNumber(senderPhone);
        if (sender != null)
        {
            if (!sender.IsActive)
            {
                throw new ApiException(ResponseCode.OrderErrorInactiveAccount);
            }

            var orderSettings = await _settingService.GetSettings<OrderSettings>(cancellationToken);
            var currentActiveOrdersCount = await _unitOfWork.OrderRepository.CountActiveOrders(sender.Id);
            if (currentActiveOrdersCount >= orderSettings.MaxActiveOrderCount)
            {
                throw new ApiException(ResponseCode.OrderErrorExceedAllowOrderCount, $"Can't create order because your account has currently had over allowed active order count: {orderSettings.MaxActiveOrderCount}");
            }
        }

        if (sender == null)
        {
            sender = new Account
            {
                Role = Role.Customer,
                Username = senderPhone,
                PhoneNumber = senderPhone
            };
            await _unitOfWork.AccountRepository.AddAsync(sender);
        }

        var receiverPhone = command.ReceiverPhone;
        Account? receiver = null;
        if (!string.IsNullOrEmpty(receiverPhone) && !Equals(senderPhone, receiverPhone))
        {
            receiver = await _unitOfWork.AccountRepository.GetCustomerByPhoneNumber(receiverPhone);
            if (receiver == null)
            {
                receiver = new Account
                {
                    Role = Role.Customer,
                    Username = receiverPhone,
                    PhoneNumber = receiverPhone
                };
                await _unitOfWork.AccountRepository.AddAsync(receiver);
            }
        }
        
        // Check delivery address
        var deliveryAddressCommand = command.DeliveryAddress;
        Location? deliveryAddress = null;
        if (Equals(command.Type, OrderType.Laundry) && deliveryAddressCommand != null)
        {
            var province = await _unitOfWork.AddressRepository
                    .Get(p => p.Code != null && p.Code.Equals(deliveryAddressCommand.ProvinceCode))
                    .FirstOrDefaultAsync(cancellationToken);
            if (province == null)
            {
                throw new ApiException(ResponseCode.AddressErrorProvinceNotFound);
            }

            var district = await _unitOfWork.AddressRepository
                    .Get(d => d.Code != null && d.Code.Equals(deliveryAddressCommand.DistrictCode))
                    .FirstOrDefaultAsync(cancellationToken);
            if (district == null || district.ParentCode != province.Code)
            {
                throw new ApiException(ResponseCode.AddressErrorDistrictNotFound);
            }

            var ward = await _unitOfWork.AddressRepository
                .Get(w => w.Code != null && w.Code.Equals(deliveryAddressCommand.WardCode))
                .FirstOrDefaultAsync(cancellationToken);
            if (ward == null || ward.ParentCode != district.Code)
            {
                throw new ApiException(ResponseCode.AddressErrorWardNotFound);
            }

            deliveryAddress = new Location()
            {
                Address = deliveryAddressCommand.Address,
                Province = province,
                District = district,
                Ward = ward,
                Longitude = deliveryAddressCommand.Longitude,
                Latitude = deliveryAddressCommand.Latitude
            };
        }

        var order = new Order
        {
            LockerId = command.LockerId,
            Type = command.Type,
            Details = details,
            Status = OrderStatus.Initialized,
            Sender = sender,
            Receiver = receiver,
            SendBox = availableBox,
            ReceiveBox = availableBox,
            DeliveryAddress = deliveryAddress
        };
        await _unitOfWork.OrderRepository.AddAsync(order);
        await _unitOfWork.SaveChangesAsync();
        
        _logger.LogInformation("Create new order: {0}", order.Id);

        // push event
        await _rabbitMqBus.PublishAsync(new OrderInitializedEvent()
        {
            OrderId = order.Id,
            Time = DateTimeOffset.UtcNow,
            Status = order.Status
        }, cancellationToken);
        
        return _mapper.Map<OrderResponse>(order);
    }
}