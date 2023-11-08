using LockerService.Application.Common.Persistence.Repositories;
using LockerService.Application.Features.Stores.Commands;

namespace LockerService.Application.Features.Stores.Handlers;

public class RemoveStoreServiceHandler : IRequestHandler<RemoveStoreServiceCommand, StatusResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    
    private readonly IResourceAuthorizeService _resourceAuthorizeService;
    
    private readonly ICurrentAccountService _currentAccountService;


    public RemoveStoreServiceHandler(IUnitOfWork unitOfWork, IResourceAuthorizeService resourceAuthorizeService, ICurrentAccountService currentAccountService)
    {
        _unitOfWork = unitOfWork;
        _resourceAuthorizeService = resourceAuthorizeService;
        _currentAccountService = currentAccountService;
    }

    public async Task<StatusResponse> Handle(RemoveStoreServiceCommand request, CancellationToken cancellationToken)
    {
        // Check store exists
        var store = await _unitOfWork.StoreRepository.GetByIdAsync(request.StoreId);
        if (store == null)
        {
            throw new ApiException(ResponseCode.StoreErrorNotFound);
        }
         
        // Check if current account can access this store
        var currentAccount = await _currentAccountService.GetRequiredCurrentAccount();
        var authorized = await _resourceAuthorizeService.AuthorizeStore(currentAccount, store);
        if (!authorized)
        {
            throw new ApiException(ResponseCode.Forbidden);
        }
        
        // Check if this store has been configured this service before
        var storeService = await _unitOfWork.StoreServiceRepository.Get(item =>
            Equals(item.StoreId, request.StoreId) && Equals(item.ServiceId, request.ServiceId))
            .FirstOrDefaultAsync(cancellationToken);

        if (storeService == null)
        {
            throw new ApiException(ResponseCode.StoreServiceErrorNotGFound);
        }

        // Remove store - service mapping in StoreService table
        await _unitOfWork.StoreServiceRepository.DeleteAsync(storeService);
        await _unitOfWork.SaveChangesAsync();

        return new StatusResponse();
    }
}