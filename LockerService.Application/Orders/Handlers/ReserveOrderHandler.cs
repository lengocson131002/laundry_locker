using LockerService.Application.EventBus.RabbitMq;
using LockerService.Application.EventBus.RabbitMq.Events.Lockers;
using LockerService.Application.EventBus.RabbitMq.Events.Orders;
using LockerService.Domain.Entities.Settings;

namespace LockerService.Application.Orders.Handlers;

public class ReserveOrderHandler : IRequestHandler<ReserveOrderCommand, OrderResponse>
{
    private readonly ILogger<InitializeOrderHandler> _logger;
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IRabbitMqBus _rabbitMqBus;
    private readonly ICurrentPrincipalService _currentPrincipalService;
    private readonly ISettingService _settingService;

    public ReserveOrderHandler(
        ILogger<InitializeOrderHandler> logger,
        IMapper mapper,
        IUnitOfWork unitOfWork,
        IRabbitMqBus rabbitMqBus, 
        ICurrentPrincipalService currentPrincipalService, 
        ISettingService settingService)
    {
        _logger = logger;
        _mapper = mapper;
        _unitOfWork = unitOfWork;
        _rabbitMqBus = rabbitMqBus;
        _currentPrincipalService = currentPrincipalService;
        _settingService = settingService;
    }

    public async Task<OrderResponse> Handle(ReserveOrderCommand command, CancellationToken cancellationToken)
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
            await _rabbitMqBus.PublishAsync(new LockerOverloadedEvent()
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
                var service = await _unitOfWork.ServiceRepository.GetStoreService(storeId: locker.StoreId, serviceId: serviceId);
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
        var senderId = _currentPrincipalService.CurrentSubjectId;
        if (senderId == null)
        {
            throw new ApiException(ResponseCode.Unauthorized);
        }
        var sender = await _unitOfWork.AccountRepository.GetCustomerById(senderId.Value);
        if (sender == null)
        {
            throw new ApiException(ResponseCode.Unauthorized);
        }
        
        var orderSettings = await _settingService.GetSettings<OrderSettings>(cancellationToken);
        var currentActiveOrdersCount = await _unitOfWork.OrderRepository.CountActiveOrders(sender.Id);
        if (currentActiveOrdersCount >= orderSettings.MaxActiveOrderCount)
        {
            throw new ApiException(
                ResponseCode.OrderErrorExceedAllowOrderCount, 
                $"Can't create order because your account has currently had over allowed active orders count: {orderSettings.MaxActiveOrderCount}");
        }
        
        var receiverPhone = command.ReceiverPhone;
        Account? receiver = null;
        if (!string.IsNullOrEmpty(receiverPhone) && !Equals(sender.PhoneNumber, receiverPhone))
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
        
        var pinCode = await _unitOfWork.OrderRepository.GenerateOrderPinCode();
        var order = new Order
        {
            LockerId = command.LockerId,
            Type = command.Type,
            Details = details,
            Status = OrderStatus.Reserved,
            Sender = sender,
            Receiver = receiver,
            SendBox = availableBox,
            ReceiveBox = availableBox,
            PinCode = pinCode,
            PinCodeIssuedAt = DateTimeOffset.UtcNow,
            DeliveryAddress = deliveryAddress
        };
        
        await _unitOfWork.OrderRepository.AddAsync(order);
        await _unitOfWork.SaveChangesAsync();
        _logger.LogInformation("Reserve new order: {0}", order.Id);

        // push event
        await _rabbitMqBus.PublishAsync(new OrderReservedEvent()
        {
            OrderId = order.Id,
            Status = order.Status,
        }, cancellationToken);
        
        return _mapper.Map<OrderResponse>(order);
    }
}