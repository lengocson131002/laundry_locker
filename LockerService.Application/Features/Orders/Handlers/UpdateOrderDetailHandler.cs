using LockerService.Application.Common.Persistence.Repositories;
using LockerService.Application.Features.Orders.Commands;
using LockerService.Application.Features.Orders.Models;

namespace LockerService.Application.Features.Orders.Handlers;

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

        if (!order.IsCollected && !order.IsProcessing)
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

        // Update order status to processing
        order.Status = OrderStatus.Processing;
        await _unitOfWork.OrderRepository.UpdateAsync(order);
        
        await _unitOfWork.SaveChangesAsync();
        
        return _mapper.Map<OrderItemResponse>(orderDetail);
    }
}