using LockerService.Application.Lockers.Queries;

namespace LockerService.Application.Lockers.Handlers;

public class GetAllLockersHandler : IRequestHandler<GetAllLockersQuery, PaginationResponse<Locker, LockerResponse>>
{
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentAccountService _currentAccountService;

    public GetAllLockersHandler(
        IUnitOfWork unitOfWork, 
        IMapper mapper, ICurrentAccountService currentAccountService)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _currentAccountService = currentAccountService;
    }

    public async Task<PaginationResponse<Locker, LockerResponse>> Handle(GetAllLockersQuery request,
        CancellationToken cancellationToken)
    {
                
        /*
         * Check current logged in user
         * if Store Staff, get only their store's locker
         */
        var currentLoggedInAccount = await _currentAccountService.GetCurrentAccount();
        if (currentLoggedInAccount != null && currentLoggedInAccount.IsStoreStaff)
        {
            request.StoreId = currentLoggedInAccount.StoreId;
        }

        var boxes = _unitOfWork.BoxRepository.Get();
        var lockersQuery = _unitOfWork.LockerRepository.Get(
                predicate: request.GetExpressions(),
                orderBy: request.GetOrder(),
                includes: new List<Expression<Func<Locker, object>>>()
                {
                    locker => locker.Location,
                    locker => locker.Location.Province,
                    locker => locker.Location.District,
                    locker => locker.Location.Ward,
                    locker => locker.Store,
                    locker => locker.OrderTypes
                },
                disableTracking: true
            ).GroupJoin(boxes, 
            locker => locker.Id, 
            box => box.LockerId, 
            (locker, boxGroup) => new
            {
                Locker = locker,
                BoxCount = boxGroup.Count(box => !box.Deleted)
            });

        var count = await lockersQuery.CountAsync(cancellationToken);
        var lockerBoxCount = await lockersQuery.ToListAsync(cancellationToken);
        var lockers = lockerBoxCount.Select(lb =>
        {
            var lo = _mapper.Map<LockerResponse>(lb.Locker);
            lo.BoxCount = lb.BoxCount;
            return lo;
        }).ToList();
        
        return new PaginationResponse<Locker, LockerResponse>(
            lockers,
            count,
            request.PageNumber,
            request.PageSize);
    }
}