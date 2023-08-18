using LockerService.Application.Dashboard.Models;
using LockerService.Application.Dashboard.Queries;

namespace LockerService.API.Controllers;

[ApiController]
[Route("/api/v1/dashboard")]
public class DashboardController : ApiControllerBase
{
    [HttpGet("overview")]
    public async Task<ActionResult<DashboardOverviewResponse>> GetDashboardOverview(DateTimeOffset? from, DateTimeOffset? to)
    {
        var request = new DashboardOverviewQuery()
        {
            From = from,
            To = to
        };
        
        return await Mediator.Send(request);
    }

    [HttpGet("orders")]
    public async Task<ActionResult<DashboardOrderResponse>> GetDashboardOrder(DateTimeOffset? from, DateTimeOffset? to)
    {
        var request = new DashboardOrderQuery()
        {
            From = from,
            To = to
        };

        return await Mediator.Send(request);
    }
}