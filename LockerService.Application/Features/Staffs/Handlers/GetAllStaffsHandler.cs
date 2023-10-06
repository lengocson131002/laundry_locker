using LockerService.Application.Common.Persistence.Repositories;
using LockerService.Application.Features.Staffs.Models;
using LockerService.Application.Features.Staffs.Queries;

namespace LockerService.Application.Features.Staffs.Handlers;

public class GetAllStaffsHandler : IRequestHandler<GetAllStaffsQuery, PaginationResponse<Account, StaffResponse>>
{
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentAccountService _currentAccountService;
    
    public GetAllStaffsHandler(
        IUnitOfWork unitOfWork,
        IMapper mapper, ICurrentAccountService currentAccountService)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _currentAccountService = currentAccountService;
    }

    public async Task<PaginationResponse<Account, StaffResponse>> Handle(GetAllStaffsQuery request,
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
        
        var staffs = await _unitOfWork.AccountRepository.GetAsync(
            predicate: request.GetExpressions(),
            includes: new List<Expression<Func<Account, object>>>()
            {
                staff => staff.Store
            },
            orderBy: request.GetOrder(),
            disableTracking: true
        );

        return new PaginationResponse<Account, StaffResponse>(
            staffs,
            request.PageNumber,
            request.PageSize,
            entity => _mapper.Map<StaffResponse>(entity));
    }
}