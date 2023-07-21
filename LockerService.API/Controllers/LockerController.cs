using LockerService.API.Attributes;
using LockerService.Application.Common.Enums;
using LockerService.Application.Lockers.Commands;
using LockerService.Application.Lockers.Models;
using LockerService.Application.Lockers.Queries;
using LockerService.Domain.Entities;
using AssignStaffCommand = LockerService.Application.Lockers.Commands.AssignStaffCommand;

namespace LockerService.API.Controllers;

[ApiController]
[Route("/api/v1/lockers")]
public class LockerController : ApiControllerBase
{
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<LockerResponse>> AddLocker([FromBody] AddLockerCommand command)
    {
        return await Mediator.Send(command);
    }

    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<PaginationResponse<Locker, LockerResponse>>> GetAllLockers(
        [FromQuery] GetAllLockersQuery request)
    {
        if (string.IsNullOrWhiteSpace(request.SortColumn))
        {
            request.SortColumn = "CreatedAt";
            request.SortDir = SortDirection.Desc;
        }

        return await Mediator.Send(request);
    }

    [HttpGet("{id:long}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<LockerDetailResponse>> GetLocker([FromRoute] long id)
    {
        var query = new GetLockerQuery
        {
            LockerId = id
        };
        return await Mediator.Send(query);
    }

    [HttpPut("{id:long}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<StatusResponse>> UpdateLocker([FromRoute] long id,
        [FromBody] UpdateLockerCommand command)
    {
        command.LockerId = id;
        await Mediator.Send(command);
        return new StatusResponse(true);
    }

    [HttpPut("{id:long}/status")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<StatusResponse>> UpdateLockerStatus([FromRoute] long id,
        [FromBody] UpdateLockerStatusCommand command)
    {
        command.LockerId = id;
        await Mediator.Send(command);
        return new StatusResponse(true);
    }

    [HttpPost("connect")]
    [ApiKey]
    public async Task<ActionResult<LockerResponse>> ConnectLocker([FromBody] ConnectLockerCommand command)
    {
        return await Mediator.Send(command);
    }

    [HttpGet("{id:long}/boxes")]
    public async Task<ActionResult<ListResponse<BoxStatus>>> GetAllBoxes([FromRoute] long id)
    {
        return await Mediator.Send(new GetAllBoxesQuery(id));
    }

    [HttpGet("{id:long}/timelines")]
    public async Task<ActionResult<PaginationResponse<LockerTimeline, LockerTimelineResponse>>> GetLockerTimeLines(
        [FromRoute] long id,
        [FromQuery] GetLockerTimelinesQuery query)
    {
        if (string.IsNullOrWhiteSpace(query.SortColumn))
        {
            query.SortColumn = "Time";
            query.SortDir = SortDirection.Desc;
        }

        query.LockerId = id;
        return await Mediator.Send(query);
    }

    [HttpPost("{id:long}/staffs")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<StatusResponse>> AssignStaff(
        [FromRoute] long id,
        [FromBody] AssignStaffCommand command)
    {
        command.LockerId = id;
        return await Mediator.Send(command);
    }

    [HttpDelete("{id:long}/staffs")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<StatusResponse>> RevokeStaff(
        [FromRoute] long id,
        [FromBody] RevokeStaffCommand command)
    {
        command.LockerId = id;
        return await Mediator.Send(command);
    }
}