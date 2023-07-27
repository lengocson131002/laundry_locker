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
         * if Staff, get only their managed lockers
         */
        var currentLoggedInAccount = await _currentAccountService.GetCurrentAccount();
        if (currentLoggedInAccount != null && Equals(currentLoggedInAccount.Role, Role.Staff))
        {
            request.StaffId = currentLoggedInAccount.Id;
        }
            
        var lockers = await _unitOfWork.LockerRepository.GetAsync(
                predicate: request.GetExpressions(),
                orderBy: request.GetOrder(),
                includes: new List<Expression<Func<Locker, object>>>()
                {
                    locker => locker.Location,
                    locker => locker.Location.Province,
                    locker => locker.Location.District,
                    locker => locker.Location.Ward,
                    locker => locker.Store
                }
            );

        return new PaginationResponse<Locker, LockerResponse>(
            lockers,
            request.PageNumber,
            request.PageSize,
            entity => _mapper.Map<LockerResponse>(entity));
    }
}