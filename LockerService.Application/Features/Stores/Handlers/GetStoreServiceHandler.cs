using LockerService.Application.Common.Persistence.Repositories;
using LockerService.Application.Features.Services.Models;
using LockerService.Application.Features.Stores.Queries;

namespace LockerService.Application.Features.Stores.Handlers;

public class GetStoreServiceHandler : IRequestHandler<GetStoreServiceQuery, ServiceDetailResponse>
{
    private readonly IUnitOfWork _unitOfWork;

    private readonly IMapper _mapper;

    public GetStoreServiceHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<ServiceDetailResponse> Handle(GetStoreServiceQuery request, CancellationToken cancellationToken)
    {
        var storeService = await _unitOfWork.StoreServiceRepository
            .Get(
                predicate: item => Equals(item.StoreId, request.StoreId) && Equals(item.ServiceId, request.ServiceId),
                includes: new List<Expression<Func<StoreService, object>>>()
                {
                    item => item.Service
                }    
            )
            .FirstOrDefaultAsync(cancellationToken);

        if (storeService == null)
        {
            throw new ApiException(ResponseCode.ServiceErrorNotFound);
        }

        var response = _mapper.Map<ServiceDetailResponse>(storeService.Service);
        response.Price = storeService.Price;

        return response;
    }
    
}