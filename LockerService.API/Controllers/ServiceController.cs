using LockerService.Application.Common.Enums;
using LockerService.Application.Services.Commands;
using LockerService.Application.Services.Models;
using LockerService.Application.Services.Queries;
using LockerService.Domain.Entities;

namespace LockerService.API.Controllers;

[ApiController]
[Route("/api/v1/services")]
public class ServiceController : ApiControllerBase
{
    [HttpPost]
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
    public async Task<ActionResult<StatusResponse>> UpdateService([FromRoute] long serviceId, [FromBody] UpdateServiceCommand command)
    {
        command.ServiceId = serviceId;
        await Mediator.Send(command);
        return new StatusResponse(true);
    }
    
    [HttpGet("{serviceId:long}")]
    public async Task<ActionResult<ServiceDetailResponse>> GetService([FromRoute] long serviceId)
    {
        var query = new GetServiceQuery(serviceId);
        return await Mediator.Send(query);
    }

    [HttpPut("{serviceId:long}/status")]
    public async Task<ActionResult<StatusResponse>> UpdateServiceStatus([FromBody] UpdateServiceCommand command)
    {
        await Mediator.Send(command);
        return new StatusResponse(true);
    }
}