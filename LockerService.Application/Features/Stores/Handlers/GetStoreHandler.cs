using LockerService.Application.Common.Persistence.Repositories;
using LockerService.Application.Features.Stores.Models;
using LockerService.Application.Features.Stores.Queries;

namespace LockerService.Application.Features.Stores.Handlers;

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
            predicate: store => store.Id == request.StoreId,
            includes: new List<Expression<Func<Store, object>>>
            {
                store => store.Location,
                store => store.Location.Province,
                store => store.Location.District,
                store => store.Location.Ward,
            },
            disableTracking: true);

        var store = storeQuery.FirstOrDefault();
        if (store is null)
        {
            throw new ApiException(ResponseCode.StoreErrorNotFound);
        }

        return _mapper.Map<StoreDetailResponse>(store);
    }
}