namespace LockerService.Application.Hardwares.Handlers;

public class RemoveHardwareHandler : IRequestHandler<RemoveHardwareCommand, StatusResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentPrincipalService _currentPrincipalService;

    public RemoveHardwareHandler(IUnitOfWork unitOfWork, ICurrentPrincipalService currentPrincipalService)
    {
        _unitOfWork = unitOfWork;
        _currentPrincipalService = currentPrincipalService;
    }

    public async Task<StatusResponse> Handle(RemoveHardwareCommand request, CancellationToken cancellationToken)
    {
        var locker = await _unitOfWork.LockerRepository.GetByIdAsync(request.LockerId);
        if (locker == null)
        {
            throw new ApiException(ResponseCode.LockerErrorNotFound);
        }

        var hardware = await _unitOfWork.HardwareRepository.GetByIdAsync(request.HardwareId);
        if (hardware == null || hardware.LockerId != locker.Id)
        {
            throw new ApiException(ResponseCode.HardwareErrorNotFound);
        }
    
        hardware.DeletedAt = DateTimeOffset.UtcNow;
        hardware.DeletedBy = _currentPrincipalService.CurrentSubjectId;
        
        await _unitOfWork.HardwareRepository.UpdateAsync(hardware);
        var result = await _unitOfWork.SaveChangesAsync();

        return new StatusResponse(result > 0);
    }
}