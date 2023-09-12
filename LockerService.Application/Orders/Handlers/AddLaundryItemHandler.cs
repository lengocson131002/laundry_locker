namespace LockerService.Application.Orders.Handlers;

public class AddLaundryItemHandler : IRequestHandler<AddLaundryItemCommand, LaundryItemResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    
    private readonly IMapper _mapper;

    public AddLaundryItemHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<LaundryItemResponse> Handle(AddLaundryItemCommand request, CancellationToken cancellationToken)
    {

        var orderDetail = await _unitOfWork.OrderDetailRepository
            .Get(detail => detail.Id == request.OrderDetailId 
                           && detail.OrderId == request.OrderId 
                           && detail.Order.IsLaundry)
            .Include(detail => detail.Order)
            .FirstOrDefaultAsync(cancellationToken);
        
        if (orderDetail == null)
        {
            throw new ApiException(ResponseCode.OrderDetailErrorNotFound);
        }
        
        if (!orderDetail.Order.IsCollected && !orderDetail.Order.IsProcessing)
        {
            throw new ApiException(ResponseCode.OrderErrorInvalidStatus);
        }
        
        var laundryItem = _mapper.Map<LaundryItem>(request);
        
        await _unitOfWork.LaundryItemRepository.AddAsync(laundryItem);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<LaundryItemResponse>(laundryItem);
    }
}