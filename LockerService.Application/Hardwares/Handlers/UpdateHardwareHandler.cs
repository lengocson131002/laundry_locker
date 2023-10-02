namespace LockerService.Application.Hardwares.Handlers;

public class UpdateHardwareHandler : IRequestHandler<UpdateHardwareCommand, HardwareResponse>
{
    private readonly ILogger<UpdateHardwareHandler> _logger;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public UpdateHardwareHandler(ILogger<UpdateHardwareHandler> logger, IUnitOfWork unitOfWork, IMapper mapper)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<HardwareResponse> Handle(UpdateHardwareCommand request, CancellationToken cancellationToken)
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
        hardware.Count = request.Count ?? hardware.Count;
        
        await _unitOfWork.HardwareRepository.UpdateAsync(hardware);
        await _unitOfWork.SaveChangesAsync();
        
        _logger.LogInformation("Updated hardware {0}", hardware.Id);
        
        return _mapper.Map<HardwareResponse>(hardware);
    }
}