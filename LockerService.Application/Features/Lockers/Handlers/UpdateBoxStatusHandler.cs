using LockerService.Application.Common.Persistence.Repositories;
using LockerService.Application.Features.Lockers.Commands;

namespace LockerService.Application.Features.Lockers.Handlers;

public class UpdateBoxStatusHandler: IRequestHandler<UpdateBoxStatusCommand, StatusResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<UpdateBoxStatusHandler> _logger;

    public UpdateBoxStatusHandler(IUnitOfWork unitOfWork, ILogger<UpdateBoxStatusHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<StatusResponse> Handle(UpdateBoxStatusCommand request, CancellationToken cancellationToken)
    {
        var lockerId = request.LockerId;
        var boxNumber = request.BoxNumber;
        
        var locker = await _unitOfWork.LockerRepository.GetByIdAsync(lockerId);
        if (locker == null)
        {
            throw new ApiException(ResponseCode.LockerErrorNotFound);
        }

        var box = await _unitOfWork.BoxRepository.FindBox(lockerId, boxNumber);
        if (box == null)
        {
            throw new ApiException(ResponseCode.LockerErrorBoxNotFound);
        }

        if (box.IsActive == request.IsActive)
        {
            throw new ApiException(ResponseCode.LockerErrorInvalidBoxStatus);
        }

        box.IsActive = request.IsActive;
        box.Description = request.Description;
        await _unitOfWork.BoxRepository.UpdateAsync(box);
        await _unitOfWork.SaveChangesAsync();
        _logger.LogInformation("Update locker's box {0} status to {1}", lockerId, request.IsActive);

        return new StatusResponse(true);
    }
}