using LockerService.Application.Common.Enums;
using LockerService.Application.Services.Commands;
using LockerService.Application.Services.Models;
using LockerService.Application.Services.Queries;
using LockerService.Domain.Entities;

namespace LockerService.API.Controllers;

[ApiController]
[Route("/api/v1/lockers/{lockerId:int}/services")]
public class ServiceController : ApiControllerBase
{
    [HttpPost]
    public async Task<ActionResult<ServiceResponse>> AddService(
        [FromRoute] int lockerId,
        [FromBody] AddServiceCommand command)
    {
        command.LockerId = lockerId;
        return await Mediator.Send(command);
    }


    [HttpGet]
    public async Task<ActionResult<PaginationResponse<Service, ServiceResponse>>> GetAllServices(
        [FromRoute] int lockerId,
        [FromQuery] GetAllServicesQuery query)
    {
        if (string.IsNullOrWhiteSpace(query.SortColumn))
        {
            query.SortColumn = "CreatedAt";
            query.SortDir = SortDirection.Desc;
        }
        
        query.LockerId = lockerId;
        return await Mediator.Send(query);
    }

    [HttpPut("{serviceId:int}")]
    public async Task<ActionResult<StatusResponse>> UpdateService(
        [FromRoute] int lockerId,
        [FromRoute] int serviceId,
        [FromBody] UpdateServiceCommand command)
    {
        command.LockerId = lockerId;
        command.ServiceId = serviceId;
        await Mediator.Send(command);
        return new StatusResponse(true);
    }
    
    [HttpGet("{serviceId:int}")]
    public async Task<ActionResult<ServiceDetailResponse>> GetService(
        [FromRoute] int lockerId,
        [FromRoute] int serviceId)
    {
        var query = new GetServiceQuery()
        {
            LockerId = lockerId,
            ServiceId = serviceId
        };
        
       return await Mediator.Send(query);
    }
    
    [HttpDelete("{serviceId:int}")]
    public async Task<ActionResult<StatusResponse>> RemoveService(
        [FromRoute] int lockerId,
        [FromRoute] int serviceId)
    {
        var query = new RemoveServiceCommand()
        {
            LockerId = lockerId,
            ServiceId = serviceId
        };
        
        return await Mediator.Send(query);
    }
}