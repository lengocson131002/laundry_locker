namespace LockerService.Application.Lockers.Handlers;

public class GetLockerTimelinesHandler : IRequestHandler<GetLockerTimelinesQuery, PaginationResponse<LockerTimeline, LockerTimelineResponse>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetLockerTimelinesHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<PaginationResponse<LockerTimeline, LockerTimelineResponse>> Handle(GetLockerTimelinesQuery request, CancellationToken cancellationToken)
    {
        var query = await _unitOfWork.LockerTimelineRepository.GetAsync(
            predicate: request.GetExpressions(),
            orderBy: request.GetOrder()
        );

        return new PaginationResponse<LockerTimeline, LockerTimelineResponse>(
            query,
            request.PageNumber,
            request.PageSize,
            timeline => _mapper.Map<LockerTimelineResponse>(timeline));
    }
}