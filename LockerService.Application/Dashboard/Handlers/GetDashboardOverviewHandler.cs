namespace LockerService.Application.Dashboard.Handlers;

public class GetDashboardOverviewHandler : IRequestHandler<DashboardOverviewQuery, DashboardOverviewResponse>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetDashboardOverviewHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<DashboardOverviewResponse> Handle(DashboardOverviewQuery request, CancellationToken cancellationToken)
    {
        var storeCount = await _unitOfWork.StoreRepository
            .Get(store => (request.From == null || store.CreatedAt >= request.From) 
                          && (request.To == null || store.CreatedAt <= request.To))
            .CountAsync(cancellationToken);

        var lockerCount = await _unitOfWork.LockerRepository
            .Get(locker => (request.From == null || locker.CreatedAt >= request.From)
                           && (request.To == null || locker.CreatedAt <= request.To))
            .CountAsync(cancellationToken);

        var staffCount = await _unitOfWork.AccountRepository.GetStaffs()
            .Where(staff => (request.From == null || staff.CreatedAt >= request.From)
                         && (request.To == null || staff.CreatedAt <= request.To))
            .CountAsync(cancellationToken);

        var customerCount = await _unitOfWork.AccountRepository.GetCustomers()
            .Where(customer => (request.From == null || customer.CreatedAt >= request.From)
                               && (request.To == null || customer.CreatedAt <= request.To))
            .CountAsync(cancellationToken);

        var serviceCount = await _unitOfWork.ServiceRepository
            .Get(service => (request.From == null || service.CreatedAt >= request.From)
                            && (request.To == null || service.CreatedAt <= request.To))
            .CountAsync(cancellationToken);
        
        return new DashboardOverviewResponse()
        {
            StoreCount = storeCount,
            StaffCount = staffCount,
            LockerCount = lockerCount,
            CustomerCount = customerCount,
            ServiceCount = serviceCount
        };
    }
}