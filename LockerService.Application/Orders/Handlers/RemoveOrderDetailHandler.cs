namespace LockerService.Application.Orders.Handlers;

public class RemoveOrderDetailHandler : IRequestHandler<RemoveOrderDetailCommand, OrderItemResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public RemoveOrderDetailHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<OrderItemResponse> Handle(RemoveOrderDetailCommand request, CancellationToken cancellationToken)
    {
        var order = await _unitOfWork.OrderRepository
            .Get(order => order.Id == request.OrderId)
            .Include(order => order.Details)
            .FirstOrDefaultAsync(cancellationToken);
        
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

        if (order.Details.Count <= 1)
        {
            throw new ApiException(ResponseCode.OrderDetailErrorRequired);
        }
        
        await _unitOfWork.OrderDetailRepository.DeleteAsync(orderDetail);
        await _unitOfWork.SaveChangesAsync();
        
        return _mapper.Map<OrderItemResponse>(orderDetail);
    }
}