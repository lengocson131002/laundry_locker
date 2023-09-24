namespace LockerService.Application.Orders.Handlers;

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
            var service = await _unitOfWork.ServiceRepository.GetStoreService(order.Locker.StoreId, detail.ServiceId);
            if (service == null || !service.IsActive)
            {
                throw new ApiException(ResponseCode.ServiceErrorNotFound);
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
                Service = service,
                Order = order,
                Quantity = detail.Quantity,
                Price = service.Price
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