using LockerService.Application.Common.Enums;
using LockerService.Application.Hardwares.Commands;
using LockerService.Application.Hardwares.Models;
using LockerService.Application.Hardwares.Queries;
using LockerService.Domain.Entities;

namespace LockerService.API.Controllers;

[ApiController]
[Route("/api/v1/lockers/{id:int}/hardwares")]
public class HardwareController : ApiControllerBase
{
    [HttpGet]
    public async Task<ActionResult<PaginationResponse<Hardware, HardwareResponse>>> GetAllHardwares(
        [FromRoute] int id,
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
        [FromRoute] int id,
        [FromBody] AddHardwareCommand command)
    {
        command.LockerId = id;
        return await Mediator.Send(command);
    }
    
    [HttpPut("{hardwareId:int}")]
    public async Task<ActionResult<StatusResponse>> UpdateHardware(
        [FromRoute] int id,        
        [FromRoute] int hardwareId,
        [FromBody] UpdateHardwareCommand command)
    {
        command.LockerId = id;
        command.HardwareId = hardwareId;
        
        return await Mediator.Send(command);
    }
    
    [HttpDelete("{hardwareId:int}")]
    public async Task<ActionResult<StatusResponse>> RemoveHardware(
        [FromRoute] int id,
        [FromRoute] int hardwareId)
    {
        var command = new RemoveHardwareCommand()
        {
            LockerId = id,
            HardwareId = hardwareId
        };
        
        return await Mediator.Send(command);
    }
    
    [HttpGet("{hardwareId:int}")]
    public async Task<ActionResult<HardwareDetailResponse>> GetHardware(
        [FromRoute] int id,
        [FromRoute] int hardwareId)
    {
        var query = new GetHardwareQuery()
        {
            LockerId = id,
            HardwareId = hardwareId
        };
        
        return await Mediator.Send(query);
    }
}