using LockerService.API.Attributes;
using LockerService.API.Common;
using LockerService.Application.Common.Enums;
using LockerService.Application.Features.Services.Commands;
using LockerService.Application.Features.Services.Models;
using LockerService.Application.Features.Services.Queries;
using LockerService.Domain.Enums;
namespace LockerService.API.Controllers;

[ApiController]
[Route("/api/v1/services")]
[ApiKey]
public class ServiceController : ApiControllerBase
{
    // Services
    [HttpPost]
    [AuthorizeRoles(Role.Admin, Role.Manager)]
    public async Task<ActionResult<ServiceResponse>> AddService([FromBody] AddServiceCommand command)
    {
        return await Mediator.Send(command);
    }

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

    [HttpPut("{serviceId:long}")]
    [AuthorizeRoles(Role.Admin, Role.Manager)]
    public async Task<ActionResult<StatusResponse>> UpdateService(
        [FromRoute] long serviceId, 
        [FromBody] UpdateServiceCommand command)
    {
        command.ServiceId = serviceId;
        
        await Mediator.Send(command);
        return new StatusResponse();
    }

    [HttpGet("{serviceId:long}")]
    public async Task<ActionResult<ServiceDetailResponse>> GetService([FromRoute] long serviceId)
    {
        var query = new GetServiceQuery(serviceId);
        return await Mediator.Send(query);
    }
    
}