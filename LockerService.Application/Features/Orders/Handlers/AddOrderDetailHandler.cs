using LockerService.Application.Common.Persistence.Repositories;
using LockerService.Application.Features.Orders.Commands;

namespace LockerService.Application.Features.Orders.Handlers;

public class AddOrderDetailHandler : IRequestHandler<AddOrderDetailCommand, StatusResponse>
{
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;

    public AddOrderDetailHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<StatusResponse> Handle(AddOrderDetailCommand request, CancellationToken cancellationToken)
    {
        var order = await _unitOfWork.OrderRepository
            .Get(order => Equals(order.Id, request.OrderId))
            .Include(order => order.Locker)
            .FirstOrDefaultAsync(cancellationToken);
        
        if (order == null) throw new ApiException(ResponseCode.OrderErrorNotFound);

        if (!order.IsCollected && !order.IsProcessing)
        {
            throw new ApiException(ResponseCode.OrderErrorInvalidStatus);
        }

        // Get service
        foreach (var detail in request.Details)
        {
            var storeService = await _unitOfWork.StoreServiceRepository.GetStoreService(order.Locker.StoreId, detail.ServiceId);
         
            // Check store support this service or not
            if (storeService == null)
            {
                throw new ApiException(ResponseCode.OrderErrorServiceIsNotAvailable);
            }
                
            // Check service status
            if (!storeService.Service.IsActive)
            {
                throw new ApiException(ResponseCode.OrderErrorServiceIsNotAvailable);
            }
            
            var existed = await _unitOfWork.OrderDetailRepository
                .Get(d => d.ServiceId == detail.ServiceId && d.OrderId == request.OrderId)
                .AnyAsync(cancellationToken);
            if (existed)
            {
                throw new ApiException(ResponseCode.OrderDetailErrorExisted);
            }

            var orderDetail = new OrderDetail()
            {
                ServiceId = storeService.ServiceId,
                Price = storeService.Price,
                OrderId = order.Id,
                Quantity = detail.Quantity
            };
            await _unitOfWork.OrderDetailRepository.AddAsync(orderDetail);
        }
        
        // Update order status to processing
        order.Status = OrderStatus.Processing;
        await _unitOfWork.OrderRepository.UpdateAsync(order);
        
        await _unitOfWork.SaveChangesAsync();

        return new StatusResponse();
    }
}