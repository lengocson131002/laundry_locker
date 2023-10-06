using LockerService.API.Attributes;
using LockerService.API.Common;
using LockerService.Application.Common.Enums;
using LockerService.Application.Features.Hardwares.Commands;
using LockerService.Application.Features.Hardwares.Models;
using LockerService.Application.Features.Hardwares.Queries;
using LockerService.Domain.Enums;

namespace LockerService.API.Controllers;

[ApiController]
[Route("/api/v1/lockers/{id:long}/hardwares")]
[AuthorizeRoles(Role.Admin, Role.Manager)]
[ApiKey]
public class HardwareController : ApiControllerBase
{
    [HttpGet]
    public async Task<ActionResult<PaginationResponse<Hardware, HardwareResponse>>> GetAllHardwares(
        [FromRoute] long id,
        [FromQuery] GetAllHardwareQuery query)
    {
        if (string.IsNullOrWhiteSpace(query.SortColumn))
        {
            query.SortColumn = "CreatedAt";
            query.SortDir = SortDirection.Desc;
        }
        
        query.LockerId = id;
        return await Mediator.Send(query);
    }

    [HttpPost]
    public async Task<ActionResult<HardwareResponse>> CreateHardware(
        [FromRoute] long id,
        [FromBody] AddHardwareCommand command)
    {
        command.LockerId = id;
        return await Mediator.Send(command);
    }
    
    [HttpPut("{hardwareId:long}")]
    public async Task<ActionResult<HardwareResponse>> UpdateHardware(
        [FromRoute] long id,        
        [FromRoute] long hardwareId,
        [FromBody] UpdateHardwareCommand command)
    {
        command.LockerId = id;
        command.HardwareId = hardwareId;
        
        return await Mediator.Send(command);
    }
    
    [HttpDelete("{hardwareId:long}")]
    public async Task<ActionResult<HardwareResponse>> RemoveHardware(
        [FromRoute] long id,
        [FromRoute] long hardwareId)
    {
        var command = new RemoveHardwareCommand()
        {
            LockerId = id,
            HardwareId = hardwareId
        };
        
        return await Mediator.Send(command);
    }
    
    [HttpGet("{hardwareId:long}")]
    public async Task<ActionResult<HardwareDetailResponse>> GetHardware(
        [FromRoute] long id,
        [FromRoute] long hardwareId)
    {
        var query = new GetHardwareQuery()
        {
            LockerId = id,
            HardwareId = hardwareId
        };
        
        return await Mediator.Send(query);
    }
}