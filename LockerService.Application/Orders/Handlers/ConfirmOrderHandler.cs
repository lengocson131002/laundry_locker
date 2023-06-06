using LockerService.Application.EventBus.RabbitMq.Events;
using MassTransit;

namespace LockerService.Application.Orders.Handlers;

public class ConfirmOrderHandler : IRequestHandler<ConfirmOrderCommand, OrderResponse>
{
    private readonly ILogger<ConfirmOrderHandler> _logger;

    private readonly IUnitOfWork _unitOfWork;

    private readonly IMapper _mapper;

    private readonly IPublishEndpoint _rabbitMqBus;


    public ConfirmOrderHandler(
        ILogger<ConfirmOrderHandler> logger,
        IUnitOfWork unitOfWork,
        IMapper mapper, IPublishEndpoint rabbitMqBus)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _rabbitMqBus = rabbitMqBus;
    }

    public async Task<OrderResponse> Handle(ConfirmOrderCommand command, CancellationToken cancellationToken)
    {
        var order = await _unitOfWork.OrderRepository.GetByIdAsync(command.Id);

        if (order == null)
        {
            throw new ApiException(ResponseCode.OrderErrorNotFound);
        }

        var currentStatus = order.Status;
        
        if (!OrderStatus.Initialized.Equals(currentStatus))
        {
            throw new ApiException(ResponseCode.OrderErrorInvalidStatus);
        }

        order.PinCode = await _unitOfWork.OrderRepository.GenerateOrderPinCode();
        
        order.PinCodeIssuedAt = DateTimeOffset.UtcNow;
        order.Status = OrderStatus.Waiting;

        _logger.LogInformation("Order Pin Code: {pinCode}", order.PinCode);
        
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
    
        // Push rabbit MQ
        await _rabbitMqBus.Publish(_mapper.Map<OrderCreatedEvent>(order), cancellationToken);
        
        return _mapper.Map<OrderResponse>(order);
    }
}