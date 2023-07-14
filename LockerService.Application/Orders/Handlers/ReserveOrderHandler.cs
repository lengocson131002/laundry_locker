using LockerService.Application.Common.Extensions;
using LockerService.Domain.Events;

namespace LockerService.Application.Orders.Handlers;

public class ReserveOrderHandler : IRequestHandler<ReserveOrderCommand, OrderResponse>
{
    private const int DefaultReservationTimeoutInMinutes = 10;
    private readonly IConfiguration _configuration;
    private readonly ILogger<CreateOrderHandler> _logger;
    private readonly IMapper _mapper;
    private readonly IMqttBus _mqttBus;
    private readonly IOrderTimeoutService _orderTimeoutService;
    private readonly IUnitOfWork _unitOfWork;

    public ReserveOrderHandler(IConfiguration configuration,
        ILogger<CreateOrderHandler> logger,
        IMapper mapper,
        IOrderTimeoutService orderTimeoutService,
        IUnitOfWork unitOfWork,
        IMqttBus mqttBus)
    {
        _configuration = configuration;
        _logger = logger;
        _mapper = mapper;
        _orderTimeoutService = orderTimeoutService;
        _unitOfWork = unitOfWork;
        _mqttBus = mqttBus;
    }

    public async Task<OrderResponse> Handle(ReserveOrderCommand command, CancellationToken cancellationToken)
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
            if (service == null || !service.IsActive)
                throw new ApiException(ResponseCode.OrderErrorServiceIsNotAvailable);

            var order = new Order
            {
                LockerId = command.LockerId,
                SendPhone = command.OrderPhone,
                ReceivePhone = !string.IsNullOrWhiteSpace(command.ReceivePhone) ? command.ReceivePhone : null,
                ReceiveAt = command.ReceiveTime,
                SendBox = (int)availableBox,
                ReceiveBox = (int)availableBox,
                Status = OrderStatus.Initialized,
                PinCode = await _unitOfWork.OrderRepository.GenerateOrderPinCode(),
                PinCodeIssuedAt = DateTimeOffset.UtcNow
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
                .AddMinutes(_configuration.GetValueOrDefault("Order:ReservationTimeOutInMinutes", DefaultReservationTimeoutInMinutes));

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