using LockerService.Application.Common.Persistence.Repositories;
using LockerService.Application.Features.Stores.Commands;

namespace LockerService.Application.Features.Stores.Handlers;

public class ConfigStoreServiceHandler : IRequestHandler<ConfigStoreServiceCommand, StatusResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IResourceAuthorizeService _resourceAuthorizeService;
    private readonly ICurrentAccountService _currentAccountService;

    public ConfigStoreServiceHandler(
        IUnitOfWork unitOfWork, 
        IResourceAuthorizeService resourceAuthorizeService, 
        ICurrentAccountService currentAccountService)
    {
        _unitOfWork = unitOfWork;
        _resourceAuthorizeService = resourceAuthorizeService;
        _currentAccountService = currentAccountService;
    }

    public async Task<StatusResponse> Handle(ConfigStoreServiceCommand request, CancellationToken cancellationToken)
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
        
        var storeServices = new List<StoreService>();
        foreach (var item in request.Services)
        {
            // Check if service exists
            var service = await _unitOfWork.ServiceRepository
                .Get(s => s.Id == item.ServiceId && s.IsStandard)
                .FirstOrDefaultAsync(cancellationToken);
            
            if (service == null)
            {
                throw new ApiException(ResponseCode.ServiceErrorNotFound);
            }

            // Check if store has configured this service before
            var existedConfig = await _unitOfWork.StoreServiceRepository
                .GetStoreService(store.Id, service.Id);

            if (existedConfig != null)
            {
                throw new ApiException(ResponseCode.StoreServiceErrorExisted);
            }
                
            // Create store service configuration
            var storeService = new StoreService(
                store.Id, 
                service.Id, 
                item.Price ?? service.Price);
            
            storeServices.Add(storeService);
            
        }
        
        if (storeServices.Any())
        {
            await _unitOfWork.StoreServiceRepository.AddRange(storeServices);
            await _unitOfWork.SaveChangesAsync();
        }

        return new StatusResponse();
    }
}