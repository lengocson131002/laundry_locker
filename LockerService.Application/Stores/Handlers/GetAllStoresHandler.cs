namespace LockerService.Application.Stores.Handlers;

public class GetAllStoresHandler : IRequestHandler<GetAllStoresQuery, PaginationResponse<Store, StoreResponse>>
{
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;

    public GetAllStoresHandler(
        IUnitOfWork unitOfWork,
        IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<PaginationResponse<Store, StoreResponse>> Handle(GetAllStoresQuery request,
        CancellationToken cancellationToken)
    {
        var stores = await _unitOfWork.StoreRepository.GetAsync(
            predicate: request.GetExpressions(),
            orderBy: request.GetOrder(),
            includes: new List<Expression<Func<Store, object>>>()
            {
                store => store.Location,
                store => store.Location.Province,
                store => store.Location.District,
                store => store.Location.Ward,
            }
        );

        return new PaginationResponse<Store, StoreResponse>(
            stores,
            request.PageNumber,
            request.PageSize,
            entity => _mapper.Map<StoreResponse>(entity));
    }
}