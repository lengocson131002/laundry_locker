namespace LockerService.Application.Orders.Handlers;

public class ProcessOrderHandler : IRequestHandler<ProcessOrderCommand, OrderResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<ProcessOrderHandler> _logger;
    private readonly IMapper _mapper;
    private readonly IMqttBus _mqttBus;
    
    public ProcessOrderHandler(
        ILogger<ProcessOrderHandler> logger, 
        IUnitOfWork unitOfWork, 
        IMapper mapper, 
        IMqttBus mqttBus)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _mqttBus = mqttBus;
    }

    public async Task<OrderResponse> Handle(ProcessOrderCommand request, CancellationToken cancellationToken)
    {
        var order = await _unitOfWork.OrderRepository.GetByIdAsync(request.Id);
        if (order == null)
        {
            throw new ApiException(ResponseCode.OrderErrorNotFound);
        }

        var currentStatus = order.Status;
        
        if (!order.CanProcess)
        {
            throw new ApiException(ResponseCode.OrderErrorInvalidStatus);
        }

        order.Status = OrderStatus.Processing;
        
        await _unitOfWork.OrderRepository.UpdateAsync(order);
        
        // Save timeline
        var timeline = new OrderTimeline()
        {
            Order = order,
            PreviousStatus = currentStatus,
            Status = order.Status,
            Time = DateTimeOffset.UtcNow
        };
        await _unitOfWork.OrderTimelineRepository.AddAsync(timeline);
        
        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation("Processing order {orderId}", order.Id);
        
        // Mqtt Open Box
        await _mqttBus.PublishAsync(new MqttOpenBoxEvent(order.LockerId, order.ReceiveBoxOrder));
        
        return _mapper.Map<OrderResponse>(order);
    }
}