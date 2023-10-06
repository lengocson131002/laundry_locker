using LockerService.API.Attributes;
using LockerService.API.Common;
using LockerService.Application.Features.Dashboard.Models;
using LockerService.Application.Features.Dashboard.Queries;
using LockerService.Domain.Enums;

namespace LockerService.API.Controllers;

[ApiController]
[Route("/api/v1/dashboard")]
[AuthorizeRoles(Role.Admin, Role.Manager)]
[ApiKey]
public class DashboardController : ApiControllerBase
{
    [HttpGet("overview")]
    public async Task<ActionResult<DashboardOverviewResponse>> GetDashboardOverview([FromQuery] DashboardOverviewQuery query)
    {
        return await Mediator.Send(query);
    }

    [HttpGet("orders")]
    public async Task<ActionResult<DashboardOrderResponse>> GetDashboardOrders([FromQuery] DashboardOrderQuery query)
    {
        return await Mediator.Send(query);
    }
    
    [HttpGet("stores")]
    public async Task<ActionResult<PaginationResponse<DashboardStoreItem>>> GetDashboardStores([FromQuery] DashboardStoreQuery query)
    {
        return await Mediator.Send(query);
    }
    
    [HttpGet("revenue")]
    public async Task<ActionResult<ListResponse<DashboardRevenueItem>>> GetDashboardRevenue([FromQuery] DashboardRevenueQuery query)
    {
        return await Mediator.Send(query);
    }
    
    [HttpGet("lockers")]
    public async Task<ActionResult<ListResponse<DashboardLockerItem>>> GetDashboardLockers([FromQuery] DashboardLockerQuery query)
    {
        return await Mediator.Send(query);
    }
    
    [HttpGet("lockers/locations")]
    public async Task<ActionResult<ListResponse<DashboardLockerLocationItem>>> GetDashboardLockerLocations([FromQuery] DashboardLockerLocationQuery query)
    {
        return await Mediator.Send(query);
    }
}