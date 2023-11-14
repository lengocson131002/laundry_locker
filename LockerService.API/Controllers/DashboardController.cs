using LockerService.API.Attributes;
using LockerService.API.Common;
using LockerService.Application.Features.Dashboard.Models;
using LockerService.Application.Features.Dashboard.Queries;
using LockerService.Domain.Enums;

namespace LockerService.API.Controllers;

/// <summary>
/// DASHBOARD API 
/// </summary>
[ApiController]
[Route("/api/v1/dashboard")]
[Authorize]
[ApiKey]
public class DashboardController : ApiControllerBase
{
    /// <summary>
    /// Get overview statistic
    /// </summary>
    /// <param name="query"></param>
    /// <returns></returns>
    [HttpGet("overview")]
    public async Task<ActionResult<DashboardOverviewResponse>> GetDashboardOverview([FromQuery] DashboardOverviewQuery query)
    {
        return await Mediator.Send(query);
    }

    /// <summary>
    /// Get dashboard order statistic
    /// </summary>
    /// <param name="query"></param>
    /// <returns></returns>
    [HttpGet("orders")]
    public async Task<ActionResult<DashboardOrderResponse>> GetDashboardOrders([FromQuery] DashboardOrderQuery query)
    {
        return await Mediator.Send(query);
    }
    
    /// <summary>
    /// Get dashboard store statistic
    /// </summary>
    /// <param name="query"></param>
    /// <returns></returns>
    [HttpGet("stores")]
    public async Task<ActionResult<PaginationResponse<DashboardStoreItem>>> GetDashboardStores([FromQuery] DashboardStoreQuery query)
    {
        return await Mediator.Send(query);
    }
    
    /// <summary>
    /// Get dashboard revenue statistic
    /// </summary>
    /// <param name="query"></param>
    /// <returns></returns>
    [HttpGet("revenue")]
    public async Task<ActionResult<ListResponse<DashboardRevenueItem>>> GetDashboardRevenue([FromQuery] DashboardRevenueQuery query)
    {
        return await Mediator.Send(query);
    }
    
    /// <summary>
    /// Get dashboard locker statistic
    /// </summary>
    /// <param name="query"></param>
    /// <returns></returns>
    [HttpGet("lockers")]
    public async Task<ActionResult<ListResponse<DashboardLockerItem>>> GetDashboardLockers([FromQuery] DashboardLockerQuery query)
    {
        return await Mediator.Send(query);
    }
    
    /// <summary>
    /// Get dashboard locker location statistic
    /// </summary>
    /// <param name="query"></param>
    /// <returns></returns>
    [HttpGet("lockers/locations")]
    public async Task<ActionResult<ListResponse<DashboardLockerLocationItem>>> GetDashboardLockerLocations([FromQuery] DashboardLockerLocationQuery query)
    {
        return await Mediator.Send(query);
    }
}