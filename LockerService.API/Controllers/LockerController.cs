using LockerService.API.Attributes;
using LockerService.API.Common;
using LockerService.Application.Common.Enums;
using LockerService.Application.Lockers.Commands;
using LockerService.Application.Lockers.Models;
using LockerService.Application.Lockers.Queries;
using LockerService.Application.Services.Models;
using LockerService.Application.Services.Queries;
using LockerService.Domain.Enums;

namespace LockerService.API.Controllers;

[ApiController]
[Route("/api/v1/lockers")]
[ApiKey]
public class LockerController : ApiControllerBase
{
    [HttpPost]
    [AuthorizeRoles(Role.Admin, Role.Manager)]
    public async Task<ActionResult<LockerResponse>> AddLocker([FromBody] AddLockerCommand command)
    {
        return await Mediator.Send(command);
    }

    [HttpGet]
    public async Task<ActionResult<PaginationResponse<Locker, LockerResponse>>> GetAllLockers(
        [FromQuery] GetAllLockersQuery query)
    {
        if (string.IsNullOrWhiteSpace(query.SortColumn))
        {
            query.SortColumn = "CreatedAt";
            query.SortDir = SortDirection.Desc;
        }

        return await Mediator.Send(query);
    }

    [HttpGet("{id:long}")]
    public async Task<ActionResult<LockerDetailResponse>> GetLocker([FromRoute] long id)
    {
        var query = new GetLockerQuery
        {
            LockerId = id
        };
        return await Mediator.Send(query);
    }

    [HttpGet("{id:long}/services")]
    public async Task<ActionResult<PaginationResponse<Service, ServiceResponse>>> GetLockerServices([FromRoute] long id,
        [FromQuery] GetAllServicesQuery query)
    {
        query.LockerId = id;

        if (string.IsNullOrWhiteSpace(query.SortColumn))
        {
            query.SortColumn = "UpdatedAt";
            query.SortDir = SortDirection.Desc;
        }

        return await Mediator.Send(query);
    }

    [HttpPost("{id:long}/staffs")]
    [AuthorizeRoles(Role.Admin, Role.Manager)]
    public async Task<ActionResult<StatusResponse>> AssignStaffs([FromRoute] long id,
        [FromBody] AssignStaffCommand command)
    {
        command.LockerId = id;
        return await Mediator.Send(command);
    }

    [HttpDelete("{id:long}/staffs")]
    [AuthorizeRoles(Role.Admin, Role.Manager)]
    public async Task<ActionResult<StatusResponse>> UnAssignStaffs([FromRoute] long id,
        [FromBody] RevokeStaffCommand command)
    {
        command.LockerId = id;
        return await Mediator.Send(command);
    }

    [HttpPut("{id:long}")]
    [AuthorizeRoles(Role.Admin, Role.Manager)]
    public async Task<ActionResult<StatusResponse>> UpdateLocker([FromRoute] long id,
        [FromBody] UpdateLockerCommand command)
    {
        command.LockerId = id;
        await Mediator.Send(command);
        return new StatusResponse();
    }

    [HttpPut("{id:long}/status")]
    [AuthorizeRoles(Role.Admin, Role.Manager)]
    public async Task<ActionResult<StatusResponse>> UpdateLockerStatus([FromRoute] long id,
        [FromBody] UpdateLockerStatusCommand command)
    {
        command.LockerId = id;
        await Mediator.Send(command);
        return new StatusResponse();
    }

    [HttpPost("connect")]
    public async Task<ActionResult<LockerResponse>> ConnectLocker([FromBody] ConnectLockerCommand command)
    {
        return await Mediator.Send(command);
    }

    [HttpPost("{id:long}/boxes")]
    [AuthorizeRoles(Role.Admin, Role.Manager)]
    public async Task<ActionResult<StatusResponse>> AddBox([FromRoute] long id, [FromBody] AddBoxCommand command)
    {
        command.LockerId = id;
        return await Mediator.Send(command);
    }

    [HttpGet("{id:long}/boxes")]
    public async Task<ActionResult<ListResponse<BoxResponse>>> GetAllBoxes([FromRoute] long id)
    {
        return await Mediator.Send(new GetAllBoxesQuery(id));
    }

    [HttpPut("{id:long}/boxes")]
    [AuthorizeRoles(Role.Admin, Role.Manager)]
    public async Task<ActionResult<StatusResponse>> UpdateBoxStatus([FromRoute] long id,
        [FromBody] UpdateBoxStatusCommand command)
    {
        command.LockerId = id;
        return await Mediator.Send(command);
    }

    [HttpGet("{id:long}/timelines")]
    public async Task<ActionResult<PaginationResponse<LockerTimeline, LockerTimelineResponse>>> GetLockerTimeLines(
        [FromRoute] long id,
        [FromQuery] GetLockerTimelinesQuery query)
    {
        if (string.IsNullOrWhiteSpace(query.SortColumn))
        {
            query.SortColumn = "CreatedAt";
            query.SortDir = SortDirection.Desc;
        }

        query.LockerId = id;
        return await Mediator.Send(query);
    }

    [HttpGet("{id:long}/statistics")]
    [AuthorizeRoles(Role.Admin, Role.Manager)]
    public async Task<ActionResult<ListResponse<LockerEventStatisticItem>>> GetLockerEventStatistic(
        [FromRoute] long id,
        [FromQuery] LockerEventStatisticQuery query)
    {
        query.LockerId = id;
        return await Mediator.Send(query);
    }
}