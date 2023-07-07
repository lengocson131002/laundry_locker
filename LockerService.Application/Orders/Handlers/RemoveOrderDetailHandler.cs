using LockerService.Application.Orders.Queries;

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
        var order = await _unitOfWork.OrderRepository.GetByIdAsync(request.OrderId);
        if (order == null)
        {
            throw new ApiException(ResponseCode.OrderErrorNotFound);
        }

        if (!order.IsProcessing)
        {
            throw new ApiException(ResponseCode.OrderErrorInvalidStatus);
        }

        var orderDetail = await _unitOfWork.OrderDetailRepository.GetByIdAsync(request.DetailId);
        if (orderDetail == null || orderDetail.OrderId != order.Id)
        {
            throw new ApiException(ResponseCode.OrderErrorDetailNotFound);
        }

        await _unitOfWork.OrderDetailRepository.DeleteAsync(orderDetail);
        await _unitOfWork.SaveChangesAsync();
        
        return _mapper.Map<OrderItemResponse>(order);
    }
}