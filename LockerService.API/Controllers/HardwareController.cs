using LockerService.API.Attributes;
using LockerService.API.Common;
using LockerService.Application.Common.Enums;
using LockerService.Application.Features.Hardwares.Commands;
using LockerService.Application.Features.Hardwares.Models;
using LockerService.Application.Features.Hardwares.Queries;
using LockerService.Domain.Enums;

namespace LockerService.API.Controllers;

/// <summary>
/// LOCKER HARDWARE API
/// </summary>
[ApiController]
[Route("/api/v1/lockers/{id:long}/hardwares")]
[AuthorizeRoles(Role.Admin, Role.Manager)]
[ApiKey]
public class HardwareController : ApiControllerBase
{
    /// <summary>
    /// Get a locker's hardwares
    /// </summary>
    /// <param name="id"></param>
    /// <param name="query"></param>
    /// <returns></returns>
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

    /// <summary>
    /// Create a locker's hardware
    /// </summary>
    /// <param name="id"></param>
    /// <param name="command"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task<ActionResult<HardwareResponse>> CreateHardware(
        [FromRoute] long id,
        [FromBody] AddHardwareCommand command)
    {
        command.LockerId = id;
        return await Mediator.Send(command);
    }
    
    /// <summary>
    /// Update a locker's hardware
    /// </summary>
    /// <param name="id"></param>
    /// <param name="hardwareId"></param>
    /// <param name="command"></param>
    /// <returns></returns>
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
    
    /// <summary>
    /// Remove a locker's hardware
    /// </summary>
    /// <param name="id"></param>
    /// <param name="hardwareId"></param>
    /// <returns></returns>
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
    
    /// <summary>
    /// Get a hardware detail
    /// </summary>
    /// <param name="id"></param>
    /// <param name="hardwareId"></param>
    /// <returns></returns>
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