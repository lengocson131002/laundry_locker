namespace LockerService.Application.Orders.Handlers;

public class UpdateOrderHandler : IRequestHandler<UpdateOrderCommand, OrderResponse>
{
    private readonly ILogger<UpdateOrderHandler> _logger;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public UpdateOrderHandler(
        ILogger<UpdateOrderHandler> logger, 
        IUnitOfWork unitOfWork, 
        IMapper mapper)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<OrderResponse> Handle(UpdateOrderCommand request, CancellationToken cancellationToken)
    {
        var order = await _unitOfWork.OrderRepository.GetByIdAsync(request.Id);
        if (order == null)
        {
            throw new ApiException(ResponseCode.OrderErrorNotFound);
        }

        if (!order.IsProcessing)
        {
            throw new ApiException(ResponseCode.OrderErrorInvalidStatus);
        }

        order.Description = request.Description ??= order.Description;

        await _unitOfWork.OrderRepository.UpdateAsync(order);
        _logger.LogInformation("update order Amount/Fee. Order ID: {orderId} ", order.Id);

        await _unitOfWork.SaveChangesAsync();
        
        return _mapper.Map<OrderResponse>(order);
    }
}