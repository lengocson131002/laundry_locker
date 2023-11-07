using LockerService.API.Attributes;
using LockerService.API.Common;
using LockerService.Application.Common.Enums;
using LockerService.Application.Features.Auth.Models;
using LockerService.Application.Features.Lockers.Commands;
using LockerService.Application.Features.Lockers.Models;
using LockerService.Application.Features.Lockers.Queries;
using LockerService.Application.Features.Services.Models;
using LockerService.Application.Features.Services.Queries;
using LockerService.Application.Features.Tokens.Models;
using LockerService.Domain.Enums;

namespace LockerService.API.Controllers;

[Route("/api/v1/lockers")]
[ApiKey]
[ApiController]
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

    /**
     * Generate token to open box
     */
    [HttpPost("{id:long}/boxes/token")]
    [AuthorizeRoles(Role.Admin, Role.Manager, Role.LaundryAttendant)]
    public async Task<ActionResult<TokenResponse>> GenerateOpenBoxToken([FromRoute] long id)
    {
        var command = new GenerateOpenBoxTokenCommand(id);
        return await Mediator.Send(command);
    }

    /**
     * Open box using gernated token
     */
    [HttpPost("{id:long}/boxes/open")]
    [AuthorizeRoles(Role.Admin, Role.Manager, Role.LaundryAttendant)]
    public async Task<ActionResult<StatusResponse>> OpenBox([FromRoute] long id, [FromBody] OpenBoxCommand command)
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