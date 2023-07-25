using LockerService.Application.EventBus.RabbitMq.Events;
using LockerService.Application.EventBus.RabbitMq.Events.Lockers;
using LockerService.Application.EventBus.RabbitMq.Events.Orders;
using LockerService.Domain.Events;
using MassTransit;

namespace LockerService.Application.Orders.Handlers;

public class ReturnOrderHandler : IRequestHandler<ReturnOrderCommand, OrderResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IMqttBus _mqttBus;
    private readonly IPublishEndpoint _rabbitMqBus;

    public ReturnOrderHandler(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        IMqttBus mqttBus, IPublishEndpoint rabbitMqBus)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _mqttBus = mqttBus;
        _rabbitMqBus = rabbitMqBus;
    }

    public async Task<OrderResponse> Handle(ReturnOrderCommand request, CancellationToken cancellationToken)
    {
        var order = await _unitOfWork.OrderRepository.GetByIdAsync(request.Id);
        
        if (order == null || !OrderType.Laundry.Equals(order.Type))
        {
            throw new ApiException(ResponseCode.OrderErrorNotFound);
        }

        if (!order.IsProcessing)
        {
            throw new ApiException(ResponseCode.OrderErrorInvalidStatus);
        }

        var lockerId = order.LockerId;
        var availableBox = await _unitOfWork.LockerRepository.FindAvailableBox(lockerId);
        if (availableBox == null)
        {
            var exception = new ApiException(ResponseCode.LockerErrorNoAvailableBox);
            await _rabbitMqBus.Publish(new LockerOverloadedEvent()
            {
                LockerId = lockerId,
                Time = DateTimeOffset.UtcNow,
                ErrorCode = exception.ErrorCode,
                Error = exception.ErrorMessage
            }, cancellationToken);

            throw exception;
        }

        var currentStatus = order.Status;
        order.ReceiveBox = availableBox;
        order.Status = OrderStatus.Returned;
        await _unitOfWork.OrderRepository.UpdateAsync(order);
        await _unitOfWork.SaveChangesAsync();

        // Push rabbit MQ
        await _rabbitMqBus.Publish(new OrderReturnedEvent()
        {
            Id = order.Id,
            PreviousStatus = currentStatus,
            Status = order.Status
        }, cancellationToken);
        
        return _mapper.Map<OrderResponse>(order);
    }
}