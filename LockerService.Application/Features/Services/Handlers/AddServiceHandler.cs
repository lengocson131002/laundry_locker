using LockerService.Application.Common.Persistence.Repositories;
using LockerService.Application.Features.Services.Commands;
using LockerService.Application.Features.Services.Models;

namespace LockerService.Application.Features.Services.Handlers;

public class AddServiceHandler : IRequestHandler<AddServiceCommand, ServiceResponse>
{
    private readonly ILogger<AddServiceHandler> _logger;
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IResourceAuthorizeService _resourceAuthorizeService;
    private readonly ICurrentAccountService _currentAccountService;

    public AddServiceHandler(
        ILogger<AddServiceHandler> logger,
        IMapper mapper,
        IUnitOfWork unitOfWork, 
        IResourceAuthorizeService resourceAuthorizeService, 
        ICurrentAccountService currentAccountService)
    {
        _logger = logger;
        _mapper = mapper;
        _unitOfWork = unitOfWork;
        _resourceAuthorizeService = resourceAuthorizeService;
        _currentAccountService = currentAccountService;
    }

    public async Task<ServiceResponse> Handle(AddServiceCommand request, CancellationToken cancellationToken)
    {
        var service = _mapper.Map<Service>(request);
        
        if (request.StoreId != null)
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
                
            // If service created by store
            // => Create store entity
            // => Config storeService entity
            var storeService = new StoreService()
            {
                Store = store,
                Service = service,
                Price = service.Price
            };

            await _unitOfWork.StoreServiceRepository.AddAsync(storeService);
        }
        
        service = await _unitOfWork.ServiceRepository.AddAsync(service);
        
        // Save changes 
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<ServiceResponse>(service);
    }
}