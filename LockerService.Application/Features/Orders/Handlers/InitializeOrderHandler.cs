using LockerService.Application.Common.Persistence.Repositories;
using LockerService.Application.EventBus.RabbitMq.Events.Lockers;
using LockerService.Application.EventBus.RabbitMq.Events.Orders;
using LockerService.Application.Features.Orders.Commands;
using LockerService.Application.Features.Orders.Models;
using LockerService.Domain.Entities.Settings;

namespace LockerService.Application.Features.Orders.Handlers;

public class InitializeOrderHandler : IRequestHandler<InitializeOrderCommand, OrderDetailResponse>
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

    public async Task<OrderDetailResponse> Handle(InitializeOrderCommand command, CancellationToken cancellationToken)
    {
        var locker = await _unitOfWork.LockerRepository
            .Get( 
                predicate: lo => lo.Id == command.LockerId, 
                includes: new List<Expression<Func<Locker, object>>>()
                {
                    locker => locker.Location,
                    locker => locker.Location.Province,
                    locker => locker.Location.District,
                    locker => locker.Location.Ward,
                    locker => locker.Store,
                    locker => locker.Store.Location,
                    locker => locker.Store.Location.Province,
                    locker => locker.Store.Location.District,
                    locker => locker.Store.Location.Ward,
                    locker => locker.OrderTypes
                })
            .FirstOrDefaultAsync(cancellationToken);

        if (locker == null)
        {
            throw new ApiException(ResponseCode.LockerErrorNotFound);
        }
    
        // Check locker order types
        if (!locker.CanSupportOrderType(command.Type))
        {
            throw new ApiException(ResponseCode.LockerErrorUnsupportedOrderType);
        }
        
        // Check Store status
        if (!locker.Store.IsActive)
        {
            throw new ApiException(ResponseCode.StoreErrorInvalidStatus);
        }
        
        // Check Locker status
        if (!locker.IsActive)
        {
            throw new ApiException(ResponseCode.LockerErrorInvalidStatus);
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

        // Check sender and receiver
        var orderSettings = await _settingService.GetSettings<OrderSettings>(cancellationToken);

        var senderPhone = command.SenderPhone;
        var sender = await _unitOfWork.AccountRepository.GetCustomerByPhoneNumber(senderPhone);
        if (sender != null)
        {
            if (!sender.IsActive)
            {
                throw new ApiException(ResponseCode.OrderErrorInactiveAccount);
            }

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
                    PhoneNumber = receiverPhone
                };
                await _unitOfWork.AccountRepository.AddAsync(receiver);
            }
        }
        
        var order = new Order
        {
            LockerId = command.LockerId,
            Type = command.Type,
            Status = command.IsReserving ? OrderStatus.Reserved : OrderStatus.Initialized,
            PinCode = command.IsReserving ? await _unitOfWork.OrderRepository.GenerateOrderPinCode() : null,
            PinCodeIssuedAt = command.IsReserving ? DateTimeOffset.UtcNow : null,
            Sender = sender,
            Receiver = receiver,
            SendBox = availableBox,
            ReceiveBox = Equals(command.Type, OrderType.Storage) ? availableBox : null,
            IntendedReceiveAt = command.IntendedReceiveAt,
            CustomerNote = command.CustomerNote
        };
        
        if (Equals(command.Type, OrderType.Laundry))
        {
            // check receive at
            if (order.IntendedReceiveAt != null && order.IntendedReceiveAt <= DateTimeOffset.UtcNow.AddHours(orderSettings.MinTimeProcessLaundryOrderInHours))
            {
                throw new ApiException(ResponseCode.OrderErrorInvalidReceiveTime);
            }
            
            if (order.IntendedReceiveAt != null)
            {
                // Thời gian quá hạn dự kiên được tính như sau:
                // Nếu người dùng chọn giờ nhận thì thời gian quá hạn dự kiến = thời gian nhận + thời gian chờ tối đa
                // Ngược lại thời gian quá hạn dự kiến = thời gian xử lý 1 đơn hàn giặt sấy + thời gian chờ tối đa 
                order.IntendedOvertime = order.IntendedReceiveAt.Value.AddHours(orderSettings.MaxTimeInHours);
                order.ExtraFee = orderSettings.ExtraFee;
            }
            
            // Check services
            var details = new List<OrderDetail>();
            foreach (var orderDetailItem in command.Details)
            {
                var serviceId = orderDetailItem.ServiceId;
                var quantity = orderDetailItem.Quantity;
                
                var storeService = await _unitOfWork.StoreServiceRepository.GetStoreService(locker.StoreId, serviceId);
            
                // Check store support this service or not
                if (storeService == null)
                {
                    throw new ApiException(ResponseCode.OrderErrorServiceIsNotAvailable);
                }
            
                // Check service status
                if (!storeService.Service.IsActive)
                {
                    throw new ApiException(ResponseCode.OrderErrorServiceIsNotAvailable);
                }

                var orderDetail = new OrderDetail
                {
                    ServiceId = serviceId,
                    Quantity = quantity,
                    Price = storeService.Price
                };

                details.Add(orderDetail);
            }
            
            await _unitOfWork.OrderDetailRepository.AddRange(details);
            order.Details = details;
            
            // Check delivery address
            var deliveryAddressCommand = command.DeliveryAddress;
            if (deliveryAddressCommand != null)
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

                var deliveryAddress = new Location()
                {
                    Address = deliveryAddressCommand.Address,
                    Province = province,
                    District = district,
                    Ward = ward,
                    Longitude = deliveryAddressCommand.Longitude,
                    Latitude = deliveryAddressCommand.Latitude
                };
                order.DeliveryAddress = deliveryAddress;
                
                var shippingFee = await _unitOfWork.ShippingPriceRepository.CalculateShippingPrice(locker.Location, deliveryAddress);
                order.ShippingFee = shippingFee;
            }
        }
        

        if (order.IsStorage)
        {
            order.StoragePrice = orderSettings.StoragePrice;
        }

        await _unitOfWork.OrderRepository.AddAsync(order);
        await _unitOfWork.SaveChangesAsync();
        
        // push event
        if (!command.IsReserving)
        {
            await _rabbitMqBus.PublishAsync(new OrderInitializedEvent()
            {
                Order = order,
                Time = DateTimeOffset.UtcNow,
            }, cancellationToken);
        }
        else
        {
            await _rabbitMqBus.PublishAsync(new OrderReservedEvent()
            {
                Order = order,
                Time = DateTimeOffset.UtcNow,
            }, cancellationToken);
        }

        _logger.LogInformation("Create new order: {0}", order.Id);

        return _mapper.Map<OrderDetailResponse>(order);

    }
}