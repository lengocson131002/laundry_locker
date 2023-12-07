using LockerService.Application.Common.Persistence.Repositories;
using LockerService.Application.Features.Dashboard.Models;
using LockerService.Application.Features.Dashboard.Queries;

namespace LockerService.Application.Features.Dashboard.Handlers;

public class GetDashboardLockerLocationHandler : IRequestHandler<DashboardLockerLocationQuery, ListResponse<DashboardLockerLocationItem>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ICurrentAccountService _currentAccountService;

    public GetDashboardLockerLocationHandler(IUnitOfWork unitOfWork, IMapper mapper, ICurrentAccountService currentAccountService)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _currentAccountService = currentAccountService;
    }

    public async Task<ListResponse<DashboardLockerLocationItem>> Handle(DashboardLockerLocationQuery request, CancellationToken cancellationToken)
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
        
        var lockers = await _unitOfWork.LockerRepository
            .Get(
                predicate: locker => (request.StoreId == null ||  request.StoreId == locker.StoreId) 
                           && (request.ProvinceCode == null || request.ProvinceCode == locker.Location.Province.Code)
                           && (request.DistrictCode == null || request.DistrictCode == locker.Location.District.Code)
                           && (request.WardCode == null || request.DistrictCode == locker.Location.Ward.Code),
                includes: new List<Expression<Func<Locker, object>>>()
                {
                    locker => locker.Location,
                    locker => locker.Location.Province,
                    locker => locker.Location.District,
                    locker => locker.Location.Ward,
                })
            .Select(locker => _mapper.Map<DashboardLockerLocationItem>(locker))
            .ToListAsync(cancellationToken);

        return new ListResponse<DashboardLockerLocationItem>(lockers);
    }
}