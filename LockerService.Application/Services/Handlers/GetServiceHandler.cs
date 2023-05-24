using LockerService.Application.Services.Queries;

namespace LockerService.Application.Services.Handlers;

public class GetServiceHandler : IRequestHandler<GetServiceQuery, ServiceDetailResponse>
{
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;

    public GetServiceHandler(IMapper mapper, IUnitOfWork unitOfWork)
    {
        _mapper = mapper;
        _unitOfWork = unitOfWork;
    }

    public async Task<ServiceDetailResponse> Handle(GetServiceQuery request, CancellationToken cancellationToken)
    {
        var locker = await _unitOfWork.LockerRepository.GetByIdAsync(request.LockerId);
        if (locker == null)
        {
            throw new ApiException(ResponseCode.LockerErrorNotFound);
        }

        var service = await _unitOfWork.ServiceRepository.GetByIdAsync(request.ServiceId);
        if (service == null || service.LockerId != locker.Id)
        {
            throw new ApiException(ResponseCode.ServiceErrorNotFound);
        }

        return _mapper.Map<ServiceDetailResponse>(service);
    }
}