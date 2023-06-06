using LockerService.Application.EventBus.RabbitMq.Consumers;
using LockerService.Application.EventBus.RabbitMq.Events;
using LockerService.Domain.Events;
using MassTransit;

namespace LockerService.Application.Orders.Handlers;

public class ReturnOrderHandler : IRequestHandler<ReturnOrderCommand, OrderResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<ReturnOrderHandler> _logger;
    private readonly IMapper _mapper;
    private readonly IMqttBus _mqttBus;
    private readonly IFeeService _feeService;
    private readonly IPublishEndpoint _rabbitMqBus;
    public ReturnOrderHandler(
        ILogger<ReturnOrderHandler> logger,
        IUnitOfWork unitOfWork,
        IMapper mapper, 
        IMqttBus mqttBus, 
        IFeeService feeService, IPublishEndpoint rabbitMqBus)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _mqttBus = mqttBus;
        _feeService = feeService;
        _rabbitMqBus = rabbitMqBus;
    }
    
    public async Task<OrderResponse> Handle(ReturnOrderCommand request, CancellationToken cancellationToken)
    {
        var orderQuery = await _unitOfWork.OrderRepository.GetAsync(
                predicate: order => order.Id == request.Id,
                includes: new List<Expression<Func<Order, object>>>()
                {
                    order => order.Service,
                    order => order.Locker 
                }
        );
        var order = orderQuery.FirstOrDefault();
        if (order == null)
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

            order.Status = OrderStatus.Returned;
            order.Fee = _feeService.CalculateFree(order);
            order.ReceiveBoxOrder = (int)availableBox;

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

            //notify user
            _logger.LogInformation("Order {orderId} returned to the locker", order.Id);

            // Mqtt Open Box
            await _mqttBus.PublishAsync(new MqttOpenBoxEvent(order.LockerId, order.ReceiveBoxOrder));

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