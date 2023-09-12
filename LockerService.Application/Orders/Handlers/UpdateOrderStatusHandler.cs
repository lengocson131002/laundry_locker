using LockerService.Application.EventBus.RabbitMq;
using LockerService.Application.EventBus.RabbitMq.Events.Lockers;
using LockerService.Application.EventBus.RabbitMq.Events.Orders;
using LockerService.Domain.Entities.Settings;

namespace LockerService.Application.Orders.Handlers;

public class UpdateOrderStatusHandler : IRequestHandler<UpdateOrderStatusCommand, OrderResponse>
{
    private readonly ILogger<UpdateOrderStatusHandler> _logger;

    private readonly IUnitOfWork _unitOfWork;

    private readonly IMapper _mapper;

    private readonly IRabbitMqBus _rabbitMqBus;

    private readonly ISettingService _settingService;

    private readonly ICurrentAccountService _currentAccountService;

    public UpdateOrderStatusHandler(
        IUnitOfWork unitOfWork, 
        IMapper mapper, 
        IRabbitMqBus rabbitMqBus, 
        ISettingService settingService, 
        ICurrentAccountService currentAccountService, 
        ILogger<UpdateOrderStatusHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _rabbitMqBus = rabbitMqBus;
        _settingService = settingService;
        _currentAccountService = currentAccountService;
        _logger = logger;
    }

    public async Task<OrderResponse> Handle(UpdateOrderStatusCommand request, CancellationToken cancellationToken)
    {
        var order = await _unitOfWork.OrderRepository.Get(order => order.Id == request.OrderId)
            .Include(order => order.Locker)
            .Include(order => order.SendBox)
            .Include(order => order.ReceiveBox)
            .Include(order => order.Sender)
            .Include(order => order.Receiver)
            .Include(order => order.Locker.Location)
            .Include(order => order.Locker.Location.Ward)
            .Include(order => order.Locker.Location.District)
            .Include(order => order.Locker.Location.Province)
            .Include(order => order.Bill)
            .Include(order => order.Details)
            .FirstOrDefaultAsync(cancellationToken);
        
        if (order == null)
        {
            throw new ApiException(ResponseCode.OrderErrorNotFound);
        }

        if (!order.CanUpdateStatus(request.OrderStatus))
        {
            throw new ApiException(ResponseCode.OrderErrorInvalidStatus);
        }

        var previousStatus = order.Status;
        
        await HandleUpdateOrderStatus(order, request.OrderStatus);

        // Update order
        await _unitOfWork.OrderRepository.UpdateAsync(order);
        await _unitOfWork.SaveChangesAsync();
        
        var currentAccount = await _currentAccountService.GetCurrentAccount(); 
        await _rabbitMqBus.PublishAsync(new OrderUpdatedStatusEvent()
        {
            OrderId = request.OrderId,
            Status = request.OrderStatus,
            PreviousStatus = previousStatus,
            StaffId = currentAccount != null && currentAccount.IsStoreStaff 
                ? currentAccount.Id
                : null,
            Image = request.Image,
            Description = request.Description,
            Time = DateTimeOffset.Now
        }, cancellationToken);

        _logger.LogInformation("Update order status to {0}", order.Status);
        
        return _mapper.Map<OrderResponse>(order);
    }

    private async Task HandleUpdateOrderStatus(Order order, OrderStatus status)
    {
        switch (status)
        {
            case OrderStatus.Waiting:
                await HandleConfirmOrder(order);
                break;
            
            case OrderStatus.Collected:
                await HandleCollectOrder(order);
                break;

            case OrderStatus.Processed:
                await HandleProcessOrder(order);
                break;
            
            case OrderStatus.Returned:
                await HandleReturnOrder(order);
                break;

            case OrderStatus.Canceled:
                await HandleCancelOrder(order);
                break;
        }
    }

    private async Task HandleConfirmOrder(Order order)
    {
        var orderSettings = await _settingService.GetSettings<OrderSettings>();
        var now = DateTimeOffset.UtcNow;
        
        order.CreatedAt = now;
        order.Status = OrderStatus.Waiting;
        order.PinCode = await _unitOfWork.OrderRepository.GenerateOrderPinCode();
        order.PinCodeIssuedAt = now;
        
        if (order.IsStorage)
        {
            order.IntendedOvertime = now.AddHours(orderSettings.MaxTimeInHours);
            order.StoragePrice = orderSettings.StoragePrice;
        }

        if (order.IsLaundry)
        {
            order.IntendedOvertime = order.IntendedReceiveAt.AddHours(orderSettings.MaxTimeInHours);
            order.ExtraFee = orderSettings.ExtraFee;
        }
    }
    
    private Task HandleCollectOrder(Order order)
    {
        order.Status = OrderStatus.Collected;
        return Task.CompletedTask;
    }
    
    private Task HandleProcessOrder(Order order)
    {
        order.Status = OrderStatus.Processed;
        return Task.CompletedTask;
    }
    
    private async Task HandleReturnOrder(Order order)
    {
        var lockerId = order.LockerId;
        var availableBox = await _unitOfWork.BoxRepository.FindAvailableBox(lockerId);
        if (availableBox == null)
        {
            var exception = new ApiException(ResponseCode.LockerErrorNoAvailableBox);
            await _rabbitMqBus.PublishAsync(new LockerOverloadedEvent()
            {
                LockerId = lockerId,
                Time = DateTimeOffset.UtcNow,
                ErrorCode = exception.ErrorCode,
                Error = exception.ErrorMessage
            });

            throw exception;
        }

        if (availableBox.Id != order.SendBoxId)
        {
            order.ReceiveBoxId = availableBox.Id;
        }
        else
        {
            order.ReceiveBoxId = order.SendBoxId;
        }
        
        order.Status = OrderStatus.Returned;
    }

    private Task HandleCancelOrder(Order order)
    {   
        order.Status = OrderStatus.Canceled;
        order.TotalPrice = order.ReservationFee;
        return Task.CompletedTask;
    }
    
}