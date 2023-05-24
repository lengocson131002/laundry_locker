using LockerService.Application.Lockers.Queries;

namespace LockerService.Application.Lockers.Handlers;

public class GetAllLockersHandler : IRequestHandler<GetAllLockersQuery, PaginationResponse<Locker, LockerResponse>>
{
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;

    public GetAllLockersHandler(
        IUnitOfWork unitOfWork, 
        IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<PaginationResponse<Locker, LockerResponse>> Handle(GetAllLockersQuery request,
        CancellationToken cancellationToken)
    {
        var lockers = await _unitOfWork.LockerRepository.GetAsync(
                predicate: request.GetExpressions(),
                orderBy: request.GetOrder(),
                includes: new List<Expression<Func<Locker, object>>>()
                {
                    locker => locker.Location,
                    locker => locker.Location.Province,
                    locker => locker.Location.District,
                    locker => locker.Location.Ward,
                }
            );

        return new PaginationResponse<Locker, LockerResponse>(
            lockers,
            request.PageNumber,
            request.PageSize,
            entity => _mapper.Map<LockerResponse>(entity));
    }
}