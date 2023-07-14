using System.ComponentModel.DataAnnotations;
using LockerService.Application.Dashboard.Models;
using LockerService.Application.Dashboard.Queries;

namespace LockerService.API.Controllers;

[ApiController]
[Route("/api/v1/dashboard/orders")]
public class DashboardController : ApiControllerBase
{
    [HttpGet]
    public async Task<ActionResult<OrderDashboardResponse>> GetOrderDashboard(
        [Required] long lockerId, 
        DateTimeOffset? from, 
        DateTimeOffset? to)
    {
        var request = new GetOrderDashboardRequest()
        {
            LockerId = lockerId,
            From = from,
            To = to
        };
        
        return await Mediator.Send(request);
    }
}