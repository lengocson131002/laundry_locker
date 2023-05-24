namespace LockerService.Application.Orders.Handlers;

public class ConfirmOrderHandler : IRequestHandler<ConfirmOrderCommand, OrderResponse>
{
    private readonly ILogger<ConfirmOrderHandler> _logger;

    private readonly IUnitOfWork _unitOfWork;

    private readonly IMapper _mapper;


    public ConfirmOrderHandler(
        ILogger<ConfirmOrderHandler> logger,
        IUnitOfWork unitOfWork,
        IMapper mapper)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
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

        return _mapper.Map<OrderResponse>(order);
    }
}