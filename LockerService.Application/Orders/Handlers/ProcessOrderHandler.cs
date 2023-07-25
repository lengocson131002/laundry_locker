using LockerService.Application.EventBus.RabbitMq.Events.Orders;
using MassTransit;

namespace LockerService.Application.Orders.Handlers;

public class ProcessOrderHandler : IRequestHandler<ProcessOrderCommand, OrderResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<ProcessOrderHandler> _logger;
    private readonly IMapper _mapper;
    private readonly IPublishEndpoint _rabbitMqBus;
    
    public ProcessOrderHandler(
        ILogger<ProcessOrderHandler> logger, 
        IUnitOfWork unitOfWork, 
        IMapper mapper, IPublishEndpoint rabbitMqBus)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _rabbitMqBus = rabbitMqBus;
    }

    public async Task<OrderResponse> Handle(ProcessOrderCommand request, CancellationToken cancellationToken)
    {
        var order = await _unitOfWork.OrderRepository.GetByIdAsync(request.Id);

        if (order == null || !OrderType.Laundry.Equals(order.Type))
        {
            throw new ApiException(ResponseCode.OrderErrorNotFound);
        }

        if (!order.CanProcess)
        {
            throw new ApiException(ResponseCode.OrderErrorInvalidStatus);
        }

        var currentStatus = order.Status;
        order.Status = OrderStatus.Processing;
        await _unitOfWork.OrderRepository.UpdateAsync(order);
        await _unitOfWork.SaveChangesAsync();

        await _rabbitMqBus.Publish(new OrderProcessingEvent()
        {
            Id = order.Id,
            PreviousStatus = currentStatus,
            Status = order.Status
        }, cancellationToken);

        return _mapper.Map<OrderResponse>(order);
    }
}