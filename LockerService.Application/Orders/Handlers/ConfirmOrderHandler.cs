using LockerService.Application.EventBus.RabbitMq.Events.Orders;
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
        IMapper mapper, 
        IPublishEndpoint rabbitMqBus)
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
        await _unitOfWork.OrderRepository.UpdateAsync(order);
        await _unitOfWork.SaveChangesAsync();

        // Push event
        await _rabbitMqBus.Publish(new OrderConfirmedEvent()
        {
            Id = order.Id,
            PreviousStatus = currentStatus,
            Status = order.Status
        }, cancellationToken);
        
        return _mapper.Map<OrderResponse>(order);
    }
}