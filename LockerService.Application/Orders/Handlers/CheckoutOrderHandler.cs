namespace LockerService.Application.Orders.Handlers;

public class CheckoutOrderHandler : IRequestHandler<CheckoutOrderCommand, OrderResponse>
{
    private readonly ILogger<CheckoutOrderHandler> _logger;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IMqttBus _mqttBus;
    private readonly IFeeService _feeService;
    
    public CheckoutOrderHandler(IUnitOfWork unitOfWork, 
        ILogger<CheckoutOrderHandler> logger, 
        IMapper mapper, IMqttBus mqttBus, 
        IFeeService feeService)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
        _mapper = mapper;
        _mqttBus = mqttBus;
        _feeService = feeService;
    }

    public async Task<OrderResponse> Handle(CheckoutOrderCommand command, CancellationToken cancellationToken)
    {
        var orderQuery = await _unitOfWork.OrderRepository.GetAsync(
            predicate: order => command.PinCode.Equals(order.PinCode)
                        && !OrderStatus.Completed.Equals(order.Status)
                        && !OrderStatus.Canceled.Equals(order.Status),
            includes: new List<Expression<Func<Order, object>>>
            {
                order => order.Service,
                order => order.Locker
            });

        var order = orderQuery.FirstOrDefault();
    
        if (order == null)
        {
            throw new ApiException(ResponseCode.OrderErrorNotFound);
        }

        var currentStatus = order.Status;
        
        if (!order.IsWaiting && !order.IsReturned)
        {
            throw new ApiException(ResponseCode.OrderErrorInvalidStatus);
        }

        order.Status = OrderStatus.Completed;
        order.ActualReceiveTime = DateTimeOffset.UtcNow;
        order.Fee = _feeService.CalculateFree(order);
        
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
        
        // Mqtt Open Box
        await _mqttBus.PublishAsync(new MqttOpenBoxEvent(order.LockerId, order.ReceiveBoxOrder));
        
        return _mapper.Map<OrderResponse>(order);
    }
}