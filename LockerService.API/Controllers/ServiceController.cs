using LockerService.API.Attributes;
using LockerService.API.Common;
using LockerService.Application.Common.Enums;
using LockerService.Application.Features.Services.Commands;
using LockerService.Application.Features.Services.Models;
using LockerService.Application.Features.Services.Queries;
using LockerService.Domain.Enums;
namespace LockerService.API.Controllers;

/// <summary>
/// API For System Services
/// </summary>
[ApiController]
[Route("/api/v1/services")]
[ApiKey]
public class ServiceController : ApiControllerBase
{
    /// <summary>
    /// Create global service
    /// </summary>
    /// <param name="command"></param>
    /// <returns></returns>
    [HttpPost]
    [AuthorizeRoles(Role.Admin)]
    public async Task<ActionResult<ServiceResponse>> AddService([FromBody] AddServiceCommand command)
    {
        return await Mediator.Send(command);
    }

    /// <summary>
    /// Get all global services
    /// </summary>
    /// <param name="query"></param>
    /// <returns></returns>
    [HttpGet]
    public async Task<ActionResult<PaginationResponse<Service, ServiceResponse>>> GetAllServices([FromQuery] GetAllServicesQuery query)
    {
        if (string.IsNullOrWhiteSpace(query.SortColumn))
        {
            query.SortColumn = "UpdatedAt";
            query.SortDir = SortDirection.Desc;
        }

        return await Mediator.Send(query);
    }

    /// <summary>
    /// Update global service
    /// </summary>
    /// <param name="serviceId"></param>
    /// <param name="command"></param>
    /// <returns></returns>
    [HttpPut("{serviceId:long}")]
    [AuthorizeRoles(Role.Admin)]
    public async Task<ActionResult<StatusResponse>> UpdateService(
        [FromRoute] long serviceId, 
        [FromBody] UpdateServiceCommand command)
    {
        command.ServiceId = serviceId;
        
        await Mediator.Send(command);

        return new StatusResponse();
    }

    /// <summary>
    /// Get a global service detail
    /// </summary>
    /// <param name="serviceId"></param>
    /// <returns></returns>
    [HttpGet("{serviceId:long}")]
    public async Task<ActionResult<ServiceDetailResponse>> GetService([FromRoute] long serviceId)
    {
        var query = new GetServiceQuery(serviceId);
        return await Mediator.Send(query);
    }
    
}