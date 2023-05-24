using LockerService.Application.Common.Extensions;
using LockerService.Domain.Events;

namespace LockerService.Application.Orders.Handlers;

public class CreateOrderHandler : IRequestHandler<CreateOrderCommand, OrderResponse>
{
    private const int DefaultOrderTimeoutInMinutes = 5;
    private readonly IConfiguration _configuration;
    private readonly ILogger<CreateOrderHandler> _logger;
    private readonly IMapper _mapper;
    private readonly IOrderTimeoutService _orderTimeoutService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMqttBus _mqttBus;

    public CreateOrderHandler(
        ILogger<CreateOrderHandler> logger,
        IConfiguration configuration,
        IMapper mapper,
        IUnitOfWork unitOfWork, IMqttBus mqttBus, IOrderTimeoutService orderTimeoutService)
    {
        _logger = logger;
        _configuration = configuration;
        _mapper = mapper;
        _unitOfWork = unitOfWork;
        _mqttBus = mqttBus;
        _orderTimeoutService = orderTimeoutService;
    }

    public async Task<OrderResponse> Handle(CreateOrderCommand command, CancellationToken cancellationToken)
    {
        var locker = await _unitOfWork.LockerRepository.GetByIdAsync(command.LockerId);
        if (locker == null) throw new ApiException(ResponseCode.LockerErrorNotFound);
            
        // Check Locker status
        if (!LockerStatus.Active.Equals(locker.Status))
            throw new ApiException(ResponseCode.LockerErrorNotActive);
        
        try
        {
            // Check available boxes
            var availableBox = await _unitOfWork.LockerRepository.FindAvailableBox(locker.Id);
            if (availableBox == null)
            {
                throw new ApiException(ResponseCode.LockerErrorNoAvailableBox);
            }

            // Check service
            var service = await _unitOfWork.ServiceRepository.GetByIdAsync(command.ServiceId);
            if (service == null || service.LockerId != locker.Id || !service.IsActive)
                throw new ApiException(ResponseCode.OrderErrorServiceIsNotAvailable);

            var order = new Order
            {
                LockerId = command.LockerId,
                ServiceId = command.ServiceId,
                OrderPhone = command.OrderPhone,
                ReceivePhone = command.ReceivePhone,
                ReceiveTime = command.ReceiveTime,
                SendBoxOrder = (int)availableBox,
                ReceiveBoxOrder = (int)availableBox,
                Status = OrderStatus.Initialized
            };

            // Save order
            var savedOrder = await _unitOfWork.OrderRepository.AddAsync(order);
        
            // Save timeline
            var timeline = new OrderTimeline()
            {
                Order = order,
                Status = order.Status,
                PreviousStatus = null,
                Time = DateTimeOffset.UtcNow
            };

            await _unitOfWork.OrderTimelineRepository.AddAsync(timeline);

            // Save changes
            await _unitOfWork.SaveChangesAsync();
            _logger.LogInformation("Create new order: {0}", order.Id);

            // Set timeout for initialized order
            var cancelTime = DateTimeOffset.Now
                .AddMinutes(_configuration.GetValueOrDefault<int>("Order:TimeoutInMinutes", DefaultOrderTimeoutInMinutes));

            await _orderTimeoutService.CancelExpiredOrder(order.Id, cancelTime);
            
            // MQTT open box
            await _mqttBus.PublishAsync(new MqttOpenBoxEvent(locker.Id, (int)availableBox));
            
            // response
            return _mapper.Map<OrderResponse>(savedOrder);
        }
        catch (ApiException ex)
        {
            if (ex.ErrorCode == (int)ResponseCode.LockerErrorNoAvailableBox)
            {
                var overloadEvent = new LockerTimeline()
                {
                    Locker = locker,
                    Status = locker.Status,
                    Event = LockerEvent.Overload,
                    Time = DateTimeOffset.UtcNow,
                    Error = ex.ErrorMessage,
                    ErrorCode = ex.ErrorCode
                };

                await _unitOfWork.LockerTimelineRepository.AddAsync(overloadEvent);
                await _unitOfWork.SaveChangesAsync();
            }
            throw;
        }
    }
}