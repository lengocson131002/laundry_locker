namespace LockerService.Application.Orders.Handlers;

public class UpdateOrderDetailHandler : IRequestHandler<UpdateOrderDetailCommand, OrderItemResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public UpdateOrderDetailHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<OrderItemResponse> Handle(UpdateOrderDetailCommand request, CancellationToken cancellationToken)
    {
        var order = await _unitOfWork.OrderRepository.GetByIdAsync(request.OrderId);
        if (order == null)
        {
            throw new ApiException(ResponseCode.OrderErrorNotFound);
        }

        if (!order.IsProcessing)
        {
            throw new ApiException(ResponseCode.OrderErrorInvalidStatus);
        }

        var orderDetail = await _unitOfWork.OrderDetailRepository.Get(
            predicate: detail => detail.Id == request.DetailId && detail.OrderId == request.OrderId,
            includes: new List<Expression<Func<OrderDetail, object>>>()
            {
                detail => detail.Service
            }
        ).FirstOrDefaultAsync(cancellationToken);
        
        if (orderDetail == null)
        {
            throw new ApiException(ResponseCode.OrderDetailErrorNotFound);
        }

        orderDetail.Quantity = request.Quantity;
        await _unitOfWork.OrderDetailRepository.UpdateAsync(orderDetail);
        await _unitOfWork.SaveChangesAsync();
        
        return _mapper.Map<OrderItemResponse>(orderDetail);
    }
}