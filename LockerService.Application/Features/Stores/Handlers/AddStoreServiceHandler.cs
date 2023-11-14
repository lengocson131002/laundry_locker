using LockerService.Application.Common.Persistence.Repositories;
using LockerService.Application.Features.Services.Models;
using LockerService.Application.Features.Stores.Commands;

namespace LockerService.Application.Features.Stores.Handlers;

public class AddStoreServiceHandler : IRequestHandler<AddStoreServiceCommand, ServiceResponse>
{
    private readonly IUnitOfWork _unitOfWork;

    private readonly IMapper _mapper;

    private readonly ICurrentAccountService _currentAccountService;

    private readonly IResourceAuthorizeService _resourceAuthorizeService;

    public AddStoreServiceHandler(
        IUnitOfWork unitOfWork, 
        IMapper mapper, 
        ICurrentAccountService currentAccountService, IResourceAuthorizeService resourceAuthorizeService)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _currentAccountService = currentAccountService;
        _resourceAuthorizeService = resourceAuthorizeService;
    }

    public async Task<ServiceResponse> Handle(AddStoreServiceCommand request, CancellationToken cancellationToken)
    {
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
            
        // => Create store entity
        var service = _mapper.Map<Service>(request);
        await _unitOfWork.ServiceRepository.AddAsync(service);

        // => Config storeService entity
        var storeService = new StoreService()
        {
            Store = store,
            Service = service,
            Price = service.Price
        };
        await _unitOfWork.StoreServiceRepository.AddAsync(storeService);
        
        // Save changes 
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<ServiceResponse>(service);
    }
}