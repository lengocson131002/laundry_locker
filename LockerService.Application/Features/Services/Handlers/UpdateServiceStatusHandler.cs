using LockerService.Application.Common.Persistence.Repositories;
using LockerService.Application.Features.Services.Commands;

namespace LockerService.Application.Features.Services.Handlers;

public class UpdateServiceStatusHandler : IRequestHandler<UpdateServiceStatusCommand>
{
    private readonly IUnitOfWork _unitOfWork;

    private readonly IResourceAuthorizeService _resourceAuthorizeService;

    private readonly ICurrentAccountService _currentAccountService;
    
    public UpdateServiceStatusHandler(IUnitOfWork unitOfWork, 
        IResourceAuthorizeService resourceAuthorizeService, 
        ICurrentAccountService currentAccountService)
    {
        _unitOfWork = unitOfWork;
        _resourceAuthorizeService = resourceAuthorizeService;
        _currentAccountService = currentAccountService;
    }

    public async Task Handle(UpdateServiceStatusCommand request, CancellationToken cancellationToken)
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

        if (request.Status.Equals(service.Status))
        {
            throw new ApiException(ResponseCode.ServiceErrorInvalidStatus);
        }

        service.Status = request.Status;
        await _unitOfWork.ServiceRepository.UpdateAsync(service);
        await _unitOfWork.SaveChangesAsync();
    }
}