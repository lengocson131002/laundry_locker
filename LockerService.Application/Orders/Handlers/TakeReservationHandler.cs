using LockerService.Application.EventBus.RabbitMq;
using LockerService.Application.EventBus.RabbitMq.Events.Lockers;
using LockerService.Application.EventBus.RabbitMq.Events.Orders;

namespace LockerService.Application.Orders.Handlers;

public class TakeReservationHandler : IRequestHandler<TakeReservationCommand, OrderResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IRabbitMqBus _rabbitMqBus;

    public TakeReservationHandler(IUnitOfWork unitOfWork, IMapper mapper, IRabbitMqBus rabbitMqBus)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _rabbitMqBus = rabbitMqBus;
    }

    public async Task<OrderResponse> Handle(TakeReservationCommand request, CancellationToken cancellationToken)
    {
        var order = await _unitOfWork.OrderRepository.GetByIdAsync(request.Id);
        if (order == null)
        {
            throw new ApiException(ResponseCode.OrderErrorNotFound);
        }

        if (!Equals(order.Status, OrderStatus.Reserved))
        {
            throw new ApiException(ResponseCode.OrderErrorInvalidStatus);
        }
        
        var availableBox = await _unitOfWork.BoxRepository.FindAvailableBox(order.LockerId);
        if (availableBox == null)
        {
            var exception = new ApiException(ResponseCode.LockerErrorNoAvailableBox);
            await _rabbitMqBus.PublishAsync(new LockerOverloadedEvent()
            {
                LockerId = order.Id,
                Time = DateTimeOffset.UtcNow,
                ErrorCode = exception.ErrorCode,
                Error = exception.ErrorMessage
            }, cancellationToken);

            throw exception;
        }

        var currentStatus = order.Status;

        // regenerate order pin code
        order.PinCode = await _unitOfWork.OrderRepository.GenerateOrderPinCode();
        order.PinCodeIssuedAt = DateTimeOffset.UtcNow;
        order.SendBox = availableBox;
        order.ReceiveBox = availableBox;
        order.Status = OrderStatus.Initialized;

        await _unitOfWork.OrderRepository.UpdateAsync(order);
        await _unitOfWork.SaveChangesAsync();

        await _rabbitMqBus.PublishAsync(new OrderUpdatedStatusEvent()
        {
            OrderId = order.Id,
            Status = order.Status,
            PreviousStatus = currentStatus,
            Time = DateTimeOffset.UtcNow
        }, cancellationToken);
        
        return _mapper.Map<OrderResponse>(order);
    }
}