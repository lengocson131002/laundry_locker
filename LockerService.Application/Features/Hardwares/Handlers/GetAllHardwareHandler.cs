using LockerService.Application.Common.Persistence.Repositories;
using LockerService.Application.Features.Hardwares.Models;
using LockerService.Application.Features.Hardwares.Queries;

namespace LockerService.Application.Features.Hardwares.Handlers;

public class GetAllHardwareHandler : IRequestHandler<GetAllHardwareQuery, PaginationResponse<Hardware, HardwareResponse>>
{
     private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;

    public GetAllHardwareHandler(IMapper mapper, IUnitOfWork unitOfWork)
    {
        _mapper = mapper;
        _unitOfWork = unitOfWork;
    }

    public async Task<PaginationResponse<Hardware, HardwareResponse>> Handle(GetAllHardwareQuery request,
        CancellationToken cancellationToken)
    {
        var resQuery = await _unitOfWork.HardwareRepository.GetAsync(
                predicate: request.GetExpressions(),
                orderBy: request.GetOrder());

        return new(
            resQuery, 
            request.PageNumber, 
            request.PageSize, 
            hardware => _mapper.Map<HardwareResponse>(hardware));
    }
}