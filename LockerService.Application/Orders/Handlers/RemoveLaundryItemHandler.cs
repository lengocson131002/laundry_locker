namespace LockerService.Application.Orders.Handlers;

public class RemoveLaundryItemHandler : IRequestHandler<RemoveLaundryItemCommand, LaundryItemResponse>
{
    private readonly IUnitOfWork _unitOfWork;

    private readonly IMapper _mapper;


    public RemoveLaundryItemHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<LaundryItemResponse> Handle(RemoveLaundryItemCommand request, CancellationToken cancellationToken)
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
        
        var orderItem = await _unitOfWork.LaundryItemRepository
            .Get(item => item.Id == request.ItemId 
                         && item.OrderDetailId == request.DetailId 
                         && item.OrderDetail.OrderId == request.OrderId
                         && item.OrderDetail.Order.IsLaundry)
            .Include(detail => detail.OrderDetail)
            .FirstOrDefaultAsync(cancellationToken);
        
        if (orderItem == null)
        {
            throw new ApiException(ResponseCode.LaundryItemErrorNotFound);
        }

        await _unitOfWork.LaundryItemRepository.DeleteAsync(orderItem);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<LaundryItemResponse>(orderItem);
    }
}