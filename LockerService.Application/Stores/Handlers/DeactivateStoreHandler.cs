using LockerService.Application.Stores.Commands;

namespace LockerService.Application.Stores.Handlers;

public class DeactivateStoreHandler : IRequestHandler<DeactivateStoreCommand, StoreResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IJwtService _jwtService;
    private readonly ILogger<DeactivateStoreHandler> _logger;
    private readonly IMapper _mapper;

    public DeactivateStoreHandler(IMapper mapper, IUnitOfWork unitOfWork, ILogger<DeactivateStoreHandler> logger,
        IJwtService jwtService)
    {
        _mapper = mapper;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<StoreResponse> Handle(DeactivateStoreCommand request, CancellationToken cancellationToken)
    {
        var storeQuery = await _unitOfWork.StoreRepository.GetAsync(
            store => store.Id == request.StoreId,
            includes: new List<Expression<Func<Store, object>>>
            {
                locker => locker.Location,
                locker => locker.Location.Province,
                locker => locker.Location.District,
                locker => locker.Location.Ward
            });

        var store = storeQuery.FirstOrDefault();
        if (store is null)
        {
            throw new ApiException(ResponseCode.StoreErrorNotFound);
        }

        if (store.Status != StoreStatus.Active)
        {
            throw new ApiException(ResponseCode.StoreErrorInvalidStatus);
        }

        store.Status = StoreStatus.Inactive;
        await _unitOfWork.StoreRepository.UpdateAsync(store);

        // Save changes
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<StoreResponse>(store);
    }
}