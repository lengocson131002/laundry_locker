using LockerService.Application.Services.Commands;

namespace LockerService.Application.Services.Handlers;

public class RemoveServiceHandler : IRequestHandler<RemoveServiceCommand, StatusResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentPrincipalService _currentPrincipal;

    public RemoveServiceHandler(IUnitOfWork unitOfWork, ICurrentPrincipalService currentPrincipal)
    {
        _unitOfWork = unitOfWork;
        _currentPrincipal = currentPrincipal;
    }

    public async Task<StatusResponse> Handle(RemoveServiceCommand request, CancellationToken cancellationToken)
    {
        var locker = await _unitOfWork.LockerRepository.GetByIdAsync(request.LockerId);
        if (locker == null)
        {
            throw new ApiException(ResponseCode.LockerErrorNotFound);
        }
        
        var service = await _unitOfWork.ServiceRepository.GetByIdAsync(request.ServiceId);
        if (service is null || !Equals(service.LockerId, request.LockerId))
        {
            throw new ApiException(ResponseCode.ServiceErrorNotFound);
        }
        
        service.DeletedAt = DateTimeOffset.UtcNow;
        service.DeletedBy = _currentPrincipal.CurrentSubjectId;

        await _unitOfWork.ServiceRepository.UpdateAsync(service);
        await _unitOfWork.SaveChangesAsync();
        
        return new StatusResponse(true);
    }
}