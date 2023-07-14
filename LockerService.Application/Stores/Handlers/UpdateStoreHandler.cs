using LockerService.Application.Stores.Commands;

namespace LockerService.Application.Stores.Handlers;

public class UpdateStoreHandler : IRequestHandler<UpdateStoreCommand, StoreResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IJwtService _jwtService;
    private readonly ILogger<UpdateStoreHandler> _logger;
    private readonly IMapper _mapper;

    public UpdateStoreHandler(IMapper mapper, IUnitOfWork unitOfWork, ILogger<UpdateStoreHandler> logger,
        IJwtService jwtService)
    {
        _mapper = mapper;
        _unitOfWork = unitOfWork;
        _logger = logger;
        _jwtService = jwtService;
    }

    public async Task<StoreResponse> Handle(UpdateStoreCommand request, CancellationToken cancellationToken)
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

        store.Image = request.Image ?? store.Image;
        store.ContactPhone = request.ContactPhone ?? store.ContactPhone;
        await _unitOfWork.StoreRepository.UpdateAsync(store);

        // Save changes
        await _unitOfWork.SaveChangesAsync();
        return _mapper.Map<StoreResponse>(store);
    }
}