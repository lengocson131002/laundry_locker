namespace LockerService.Application.Hardwares.Handlers;

public class UpdateHardwareHandler : IRequestHandler<UpdateHardwareCommand, StatusResponse>
{
    private readonly ILogger<UpdateHardwareHandler> _logger;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateHardwareHandler(ILogger<UpdateHardwareHandler> logger, IUnitOfWork unitOfWork)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
    }

    public async Task<StatusResponse> Handle(UpdateHardwareCommand request, CancellationToken cancellationToken)
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

        hardware.Name = request.Name ?? hardware.Name;
        hardware.Code = request.Code ?? hardware.Code;
        hardware.Brand = request.Brand ?? hardware.Brand;
        hardware.Description = request.Description ?? hardware.Description;
        hardware.Price = request.Price ?? hardware.Price;

        await _unitOfWork.HardwareRepository.UpdateAsync(hardware);
        var result = await _unitOfWork.SaveChangesAsync();
        
        _logger.LogInformation("Updated hardware {0}", hardware.Id);
        
        return new StatusResponse(result > 0);
    }
}