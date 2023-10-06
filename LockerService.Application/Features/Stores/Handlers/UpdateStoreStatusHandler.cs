using LockerService.Application.Common.Persistence.Repositories;
using LockerService.Application.Features.Stores.Commands;
using LockerService.Application.Features.Stores.Models;

namespace LockerService.Application.Features.Stores.Handlers;

public class UpdateStoreStatusHandler : IRequestHandler<UpdateStoreStatusCommand, StoreResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IJwtService _jwtService;
    private readonly ILogger<UpdateStoreStatusHandler> _logger;
    private readonly IMapper _mapper;

    public UpdateStoreStatusHandler(IMapper mapper, IUnitOfWork unitOfWork, ILogger<UpdateStoreStatusHandler> logger,
        IJwtService jwtService)
    {
        _mapper = mapper;
        _unitOfWork = unitOfWork;
        _logger = logger;
        _jwtService = jwtService;
    }

    public async Task<StoreResponse> Handle(UpdateStoreStatusCommand request, CancellationToken cancellationToken)
    {
        var storeQuery = await _unitOfWork.StoreRepository.GetAsync(
            store => store.Id == request.StoreId);
        var store = storeQuery.FirstOrDefault();
        if (store is null)
        {
            throw new ApiException(ResponseCode.StoreErrorNotFound);
        }

        if (Equals(store.Status, request.Status))
        {
            throw new ApiException(ResponseCode.StoreErrorInvalidStatus);
        }

        store.Status = request.Status;
        await _unitOfWork.StoreRepository.UpdateAsync(store);

        // Save changes
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<StoreResponse>(store);
    }
}