using LockerService.Application.Common.Persistence.Repositories;
using LockerService.Application.Features.Services.Commands;

namespace LockerService.Application.Features.Services.Handlers;

public class UpdateServiceHandler : IRequestHandler<UpdateServiceCommand>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IResourceAuthorizeService _resourceAuthorizeService;
    private readonly ICurrentAccountService _currentAccountService;
    
    public UpdateServiceHandler(
        IUnitOfWork unitOfWork, 
        ICurrentAccountService currentAccountService, 
        IResourceAuthorizeService resourceAuthorizeService)
    {
        _unitOfWork = unitOfWork;
        _currentAccountService = currentAccountService;
        _resourceAuthorizeService = resourceAuthorizeService;
    }


    public async Task Handle(UpdateServiceCommand request, CancellationToken cancellationToken)
    {
        var service = await _unitOfWork.ServiceRepository.GetByIdAsync(request.ServiceId);
        
        if (service is null)
        {
            throw new ApiException(ResponseCode.ServiceErrorNotFound);
        }
        
        // Check if current account can access this services
        var currentAccount = await _currentAccountService.GetRequiredCurrentAccount();
        var authorized = await _resourceAuthorizeService.AuthorizeService(currentAccount, service);
        if (!authorized)
        {
            throw new ApiException(ResponseCode.Forbidden);
        }

        service.Name = request.Name ?? service.Name;
        service.Price = request.Price ?? service.Price;
        service.Description = request.Description ?? service.Description;
        service.Image = request.Image ?? service.Image;
        service.Unit = request.Unit ?? service.Unit;
        service.Status = request.Status ?? service.Status;
        
        await _unitOfWork.ServiceRepository.UpdateAsync(service);

        // Update store service price configuration if this service is belong to specific store
        if (!service.IsStandard)
        {
            // Check if this store has been configured this service before
            var storeService = await _unitOfWork.StoreServiceRepository
                .Get(item => Equals(item.StoreId, service.StoreId) && Equals(item.ServiceId, service.Id))
                .FirstOrDefaultAsync(cancellationToken);

            if (storeService != null)
            {
                storeService.Price = service.Price;
                await _unitOfWork.StoreServiceRepository.UpdateAsync(storeService);
            }
        }
        
        // Save changes
        await _unitOfWork.SaveChangesAsync();
    }
}