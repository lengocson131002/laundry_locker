using LockerService.Application.Audits.Models;
using LockerService.Application.Audits.Queries;

namespace LockerService.Application.Audits.Handlers;

public class GetAuditsHandler : IRequestHandler<GetAuditsQuery, PaginationResponse<Audit, AuditResponse>>
{
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;

    public GetAuditsHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<PaginationResponse<Audit, AuditResponse>> Handle(GetAuditsQuery request,
        CancellationToken cancellationToken)
    {
        var audits = await _unitOfWork.AuditRepository.GetAsync(
            request.GetExpressions(),
            request.GetOrder(),
            disableTracking: true);

        return new PaginationResponse<Audit, AuditResponse>(
            audits,
            request.PageNumber,
            request.PageSize, 
            audit => _mapper.Map<AuditResponse>(audit));
    }
}