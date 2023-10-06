using LockerService.Application.Features.Dashboard.Models;

namespace LockerService.Application.Features.Dashboard.Queries;

public class DashboardLockerLocationQuery : IRequest<ListResponse<DashboardLockerLocationItem>>
{
    public long? StoreId { get; set; }
    
    public string? ProvinceCode { get; set; }
    
    public string? DistrictCode { get; set; }
    
    public string? WardCode { get; set; }
}