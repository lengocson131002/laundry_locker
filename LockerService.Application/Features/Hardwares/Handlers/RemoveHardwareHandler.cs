using LockerService.Application.Common.Persistence.Repositories;
using LockerService.Application.Features.Hardwares.Commands;
using LockerService.Application.Features.Hardwares.Models;

namespace LockerService.Application.Features.Hardwares.Handlers;

public class RemoveHardwareHandler : IRequestHandler<RemoveHardwareCommand, HardwareResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentPrincipalService _currentPrincipalService;
    private readonly IMapper _mapper;
    
    public RemoveHardwareHandler(IUnitOfWork unitOfWork, ICurrentPrincipalService currentPrincipalService, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _currentPrincipalService = currentPrincipalService;
        _mapper = mapper;
    }

    public async Task<HardwareResponse> Handle(RemoveHardwareCommand request, CancellationToken cancellationToken)
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
    
        await _unitOfWork.HardwareRepository.DeleteAsync(hardware);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<HardwareResponse>(hardware);
    }
}