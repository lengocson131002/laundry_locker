using LockerService.Domain.Events;

namespace LockerService.Application.Orders.Handlers;

public class ReturnOrderHandler : IRequestHandler<ReturnOrderCommand, OrderResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<ReturnOrderHandler> _logger;
    private readonly IMapper _mapper;
    private readonly IMqttBus _mqttBus;
    private readonly IFeeService _feeService;
    
    public ReturnOrderHandler(
        ILogger<ReturnOrderHandler> logger,
        IUnitOfWork unitOfWork,
        IMapper mapper, 
        IMqttBus mqttBus, 
        IFeeService feeService)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _mqttBus = mqttBus;
        _feeService = feeService;
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
            
            order.ReceiveBoxOrder = (int)availableBox;
            order.Status = OrderStatus.Returned;
            order.Amount ??= request.Amount;
            order.Fee ??= request.Fee;
            order.Description ??= request.Description;
            
            // Calculate fee
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

            //notify user
            _logger.LogInformation("Order {orderId} returned to the locker", order.Id);

            // Mqtt Open Box
            await _mqttBus.PublishAsync(new MqttOpenBoxEvent(order.LockerId, order.ReceiveBoxOrder));

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