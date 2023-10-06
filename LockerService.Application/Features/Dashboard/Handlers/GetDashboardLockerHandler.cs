using LockerService.Application.Common.Persistence.Repositories;
using LockerService.Application.Features.Dashboard.Models;
using LockerService.Application.Features.Dashboard.Queries;

namespace LockerService.Application.Features.Dashboard.Handlers;

public class GetDashboardLockerHandler : IRequestHandler<DashboardLockerQuery, ListResponse<DashboardLockerItem>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetDashboardLockerHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ListResponse<DashboardLockerItem>> Handle(DashboardLockerQuery request, CancellationToken cancellationToken)
    {
        var lockerStatuses = await _unitOfWork.LockerRepository
            .Get(locker => request.StoreId == null || locker.StoreId == request.StoreId)
            .GroupBy(locker => locker.Status)
            .Select(item => new DashboardLockerItem()
            {
                Status = item.Key,
                Count = item.Count()
            }).ToListAsync(cancellationToken);

        foreach (var status in Enum.GetValues(typeof(LockerStatus)).Cast<LockerStatus>())
        {
            if (lockerStatuses.All(item => !Equals(item.Status, status)))
            {
                lockerStatuses.Add(new DashboardLockerItem(status));
            }
        }

        return new ListResponse<DashboardLockerItem>(lockerStatuses);
    }
}