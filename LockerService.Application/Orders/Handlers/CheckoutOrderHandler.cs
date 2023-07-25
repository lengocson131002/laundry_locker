using LockerService.Application.EventBus.RabbitMq.Events.Orders;
using MassTransit;

namespace LockerService.Application.Orders.Handlers;

public class CheckoutOrderHandler : IRequestHandler<CheckoutOrderCommand, OrderResponse>
{
    private readonly ILogger<CheckoutOrderHandler> _logger;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IMqttBus _mqttBus;
    private readonly IPublishEndpoint _rabbitMqBus;
    
    public CheckoutOrderHandler(IUnitOfWork unitOfWork, 
        ILogger<CheckoutOrderHandler> logger, 
        IMapper mapper, IMqttBus mqttBus, 
        IPublishEndpoint rabbitMqBus)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
        _mapper = mapper;
        _mqttBus = mqttBus;
        _rabbitMqBus = rabbitMqBus;
    }

    public async Task<OrderResponse> Handle(CheckoutOrderCommand command, CancellationToken cancellationToken)
    {
        var order = await _unitOfWork.OrderRepository.GetByIdAsync(command.Id);
        if (order == null)
        {
            throw new ApiException(ResponseCode.OrderErrorNotFound);
        }
        
        if (!order.IsWaiting && !order.IsReturned)
        {
            throw new ApiException(ResponseCode.OrderErrorInvalidStatus);
        }

        var currentStatus = order.Status;
        order.Status = OrderStatus.Completed;
        order.ReceiveAt = DateTimeOffset.UtcNow;
        await _unitOfWork.OrderRepository.UpdateAsync(order);
        await _unitOfWork.SaveChangesAsync();
        
        await _rabbitMqBus.Publish(new OrderCompletedEvent()
        {
            Id = order.Id,
            PreviousStatus = currentStatus,
            Status = order.Status
        });
        
        return _mapper.Map<OrderResponse>(order);
    }
}