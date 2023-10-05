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
        var order = await _unitOfWork.OrderRepository.Get(order => order.Id == request.Id)
            .Include(order => order.Locker)
            .Include(order => order.SendBox)
            .Include(order => order.ReceiveBox)
            .Include(order => order.Sender)
            .Include(order => order.Receiver)
            .Include(order => order.Locker.Location)
            .Include(order => order.Locker.Location.Ward)
            .Include(order => order.Locker.Location.District)
            .Include(order => order.Locker.Location.Province)
            .Include(order => order.Details)
            .FirstOrDefaultAsync(cancellationToken);
        
        if (order == null)
        {
            throw new ApiException(ResponseCode.OrderErrorNotFound);
        }

        if (!Equals(order.Status, OrderStatus.Reserved))
        {
            throw new ApiException(ResponseCode.OrderErrorInvalidStatus);
        }
        
        var currentStatus = order.Status;

        // regenerate order pin code
        order.PinCode = await _unitOfWork.OrderRepository.GenerateOrderPinCode();
        order.PinCodeIssuedAt = DateTimeOffset.UtcNow;
        order.Status = OrderStatus.Initialized;

        await _unitOfWork.OrderRepository.UpdateAsync(order);
        await _unitOfWork.SaveChangesAsync();

        await _rabbitMqBus.PublishAsync(new OrderInitializedEvent()
        {
            Order = order,
            PreviousStatus = currentStatus,
            Time = DateTimeOffset.UtcNow
        }, cancellationToken);
        
        return _mapper.Map<OrderResponse>(order);
    }
}