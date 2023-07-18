namespace LockerService.Application.Stores.Handlers;

public class GetStoreHandler : IRequestHandler<GetStoreQuery, StoreDetailResponse>
{
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;

    public GetStoreHandler(
        IUnitOfWork unitOfWork,
        IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<StoreDetailResponse> Handle(GetStoreQuery request,
        CancellationToken cancellationToken)
    {
        var storeQuery = await _unitOfWork.StoreRepository.GetAsync(
            store => store.Id == request.StoreId,
            includes: new List<Expression<Func<Store, object>>>
            {
                store => store.Location,
                store => store.Location.Province,
                store => store.Location.District,
                store => store.Location.Ward,
            });

        var store = storeQuery.FirstOrDefault();
        if (store is null)
        {
            throw new ApiException(ResponseCode.StoreErrorNotFound);
        }

        return _mapper.Map<StoreDetailResponse>(store);
    }
}