using LockerService.Application.Common.Persistence.Repositories;
using LockerService.Application.Features.Services.Models;
using LockerService.Application.Features.Services.Queries;

namespace LockerService.Application.Features.Services.Handlers;

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
        var service = await _unitOfWork.ServiceRepository.GetByIdAsync(request.ServiceId);
        
        if (service is null)
        {
            throw new ApiException(ResponseCode.ServiceErrorNotFound);
        }

        return _mapper.Map<ServiceDetailResponse>(service);
    }
}