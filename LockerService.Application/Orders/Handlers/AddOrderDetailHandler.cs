namespace LockerService.Application.Orders.Handlers;

public class AddOrderDetailHandler : IRequestHandler<AddOrderDetailCommand, OrderItemResponse>
{
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;

    public AddOrderDetailHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<OrderItemResponse> Handle(AddOrderDetailCommand request, CancellationToken cancellationToken)
    {
        var order = await _unitOfWork.OrderRepository.GetByIdAsync(request.OrderId);
        if (order == null) throw new ApiException(ResponseCode.OrderErrorNotFound);

        if (!order.IsProcessing)
        {
            throw new ApiException(ResponseCode.OrderErrorInvalidStatus);
        }

        // Get service
        var service = await _unitOfWork.ServiceRepository.GetByIdAsync(request.ServiceId);
        if (service == null || !service.IsActive)
        {
            throw new ApiException(ResponseCode.ServiceErrorNotFound);
        }

        var existed = await _unitOfWork.OrderDetailRepository
            .Get(detail => detail.ServiceId == request.ServiceId && detail.OrderId == request.OrderId)
            .AnyAsync(cancellationToken);
        if (existed)
        {
            throw new ApiException(ResponseCode.OrderDetailExisted);
        }

        var orderDetail = new OrderDetail()
        {
            Service = service,
            Order = order,
            Quantity = request.Quantity,
            Price = service.Price
        };
        await _unitOfWork.OrderDetailRepository.AddAsync(orderDetail);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<OrderItemResponse>(orderDetail);
    }
}