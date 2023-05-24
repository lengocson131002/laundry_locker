using LockerService.Application.Hardwares.Queries;

namespace LockerService.Application.Hardwares.Handlers;

public class GetHardwareHandler : IRequestHandler<GetHardwareQuery, HardwareDetailResponse>
{
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;

    public GetHardwareHandler(IMapper mapper, IUnitOfWork unitOfWork)
    {
        _mapper = mapper;
        _unitOfWork = unitOfWork;
    }

    public async Task<HardwareDetailResponse> Handle(GetHardwareQuery request, CancellationToken cancellationToken)
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

        return _mapper.Map<HardwareDetailResponse>(hardware);
    }
}