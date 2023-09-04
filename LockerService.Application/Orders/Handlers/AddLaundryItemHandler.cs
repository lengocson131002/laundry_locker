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
        var order = await _unitOfWork.OrderRepository
            .Get(order => order.Id == request.OrderId && Equals(order.Type, OrderType.Laundry))
            .FirstOrDefaultAsync(cancellationToken);

        if (order == null)
        {
            throw new ApiException(ResponseCode.OrderErrorNotFound);
        }

        if (!order.IsProcessing)
        {
            throw new ApiException(ResponseCode.OrderErrorInvalidStatus);
        }

        var laundryItem = _mapper.Map<LaundryItem>(request);
        await _unitOfWork.LaundryItemRepository.AddAsync(laundryItem);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<LaundryItemResponse>(laundryItem);
    }
}