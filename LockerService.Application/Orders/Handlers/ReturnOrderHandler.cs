using LockerService.Application.EventBus.RabbitMq.Events;
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
        var order = await _unitOfWork.OrderRepository.GetOrderByPinCode(request.PinCode);
        if (order == null || !OrderType.Laundry.Equals(order.Type))
        {
            throw new ApiException(ResponseCode.OrderErrorNotFound);
        }

        var currentStatus = order.Status;
        if (!order.IsProcessing)
        {
            throw new ApiException(ResponseCode.OrderErrorInvalidStatus);
        }

        try
        {
            var availableBox = await _unitOfWork.LockerRepository.FindAvailableBox(order.LockerId);
            if (availableBox == null)
            {
                throw new ApiException(ResponseCode.LockerErrorNoAvailableBox);
            }

            order.ReceiveBox = (int)availableBox;
            order.Status = OrderStatus.Returned;
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
            await _mqttBus.PublishAsync(new MqttOpenBoxEvent(order.LockerId, order.ReceiveBox));

            // Push rabbit MQ
            await _rabbitMqBus.Publish(_mapper.Map<OrderReturnedEvent>(order), cancellationToken);
            return _mapper.Map<OrderResponse>(order);
        }
        catch (ApiException ex)
        {
            if (ex.ErrorCode == (int)ResponseCode.LockerErrorNoAvailableBox)
            {
                var locker = order.Locker;
                var overloadEvent = new LockerTimeline()
                {
                    Locker = locker,
                    Status = locker.Status,
                    Event = LockerEvent.Overload,
                    Time = DateTimeOffset.UtcNow
                };

                await _unitOfWork.LockerTimelineRepository.AddAsync(overloadEvent);
                await _unitOfWork.SaveChangesAsync();
            }

            throw;
        }
    }
}