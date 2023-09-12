using System.Linq.Dynamic.Core;

namespace LockerService.Application.Dashboard.Handlers;

public class GetDashboardStoreHandler : IRequestHandler<DashboardStoreQuery, PaginationResponse<DashboardStoreItem>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetDashboardStoreHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<PaginationResponse<DashboardStoreItem>> Handle(DashboardStoreQuery request, CancellationToken cancellationToken)
    {
        var lockers = await _unitOfWork.LockerRepository
            .GetAsync(locker => (request.From == null || locker.CreatedAt >= request.From) && (request.To == null || locker.CreatedAt <= request.To));
        
        var staffs = _unitOfWork.AccountRepository
            .GetStaffs()
            .Where(staff => (request.From == null || staff.CreatedAt >= request.From) && (request.To == null || staff.CreatedAt <= request.To));;
        
        var orders = await _unitOfWork.OrderRepository
            .GetAsync(order => order.IsCompleted && (request.From == null || order.CreatedAt >= request.From) && (request.To == null || order.CreatedAt <= request.To));
        
        var stores = _unitOfWork.StoreRepository.Get();

        var storeQuery = from store in stores
            join locker in lockers on store.Id equals locker.StoreId into lockerGroup
            from l in lockerGroup.DefaultIfEmpty()
            join staff in staffs on store.Id equals staff.StoreId into staffGroup
            from s in staffGroup.DefaultIfEmpty()
            join order in orders on store.Id equals order.Locker.StoreId into orderGroup
            from o in orderGroup.DefaultIfEmpty()
            group new
            {
                LockerId = l.Id,
                StaffId = s.Id,
                OrderId = o.Id
            } by new { store.Id, store.Name, store.Image, store.Status, store.CreatedAt, store.UpdatedAt }
            into storeGroup
            select new DashboardStoreItem()
            {
                Id = storeGroup.Key.Id,
                Name = storeGroup.Key.Name,
                Image = storeGroup.Key.Image,
                Status = storeGroup.Key.Status,
                CreatedAt = storeGroup.Key.CreatedAt,
                UpdatedAt = storeGroup.Key.UpdatedAt,
                StaffCount = storeGroup.Select(g => g.StaffId).Distinct().Count(),
                LockerCount = storeGroup.Select(g => g.LockerId).Distinct().Count(),
                OrderCount = storeGroup.Select(g => g.OrderId).Distinct().Count(),
            };
        
        // Calculate revenue
        var revenueQuery = from store in stores
            join order in orders on store.Id equals order.Locker.StoreId into orderGroup
            from o in orderGroup.DefaultIfEmpty()
            group new
            {
                OrderId = o.Id,
                Revenue = o.TotalPrice
            } by new { store.Id, store.Name }
            into storeGroup
            select new
            {
                StoreId = storeGroup.Key.Id,
                Revenue = storeGroup.Select(g => g.Revenue).Sum()
            };

        storeQuery = from store in storeQuery
            join revenue in revenueQuery on store.Id equals revenue.StoreId into revenueGroup
            from r in revenueGroup.DefaultIfEmpty()
            select new DashboardStoreItem()
            {
                Id = store.Id,
                Name = store.Name,
                Image = store.Image,
                Status = store.Status,
                CreatedAt = store.CreatedAt,
                UpdatedAt = store.UpdatedAt,
                StaffCount = store.StaffCount,
                LockerCount = store.LockerCount,
                OrderCount = store.OrderCount,
                Revenue = r.Revenue
            };

        storeQuery = storeQuery.Where(request.GetExpressions());
        var orderBy = request.GetDynamicOrder();
        if (orderBy != null)
        {
            storeQuery = storeQuery.OrderBy(orderBy);
        }
        
        return new PaginationResponse<DashboardStoreItem>(storeQuery, request.PageNumber, request.PageSize);
    }
}