using LockerService.Application.Common.Enums;
using LockerService.Application.Lockers.Commands;
using LockerService.Application.Lockers.Models;
using LockerService.Application.Lockers.Queries;
using LockerService.Domain.Entities;

namespace LockerService.API.Controllers;

[ApiController]
[Route("/api/v1/lockers")]
public class LockerController : ApiControllerBase
{
    [HttpPost]
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

    [HttpGet("{id:int}")]
    public async Task<ActionResult<LockerDetailResponse>> GetLocker([FromRoute] int id)
    {
        var query = new GetLockerQuery
        {
            LockerId = id
        };
        return await Mediator.Send(query);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<StatusResponse>> UpdateLocker([FromRoute] int id,
        [FromBody] UpdateLockerCommand command)
    {
        command.LockerId = id;
        await Mediator.Send(command);
        return new StatusResponse(true);
    }

    [HttpPut("{id:int}/status")]
    public async Task<ActionResult<StatusResponse>> UpdateLockerStatus([FromRoute] int id,
        [FromBody] UpdateLockerStatusCommand command)
    {
        command.LockerId = id;
        await Mediator.Send(command);
        return new StatusResponse(true);
    }
    
    [HttpPost("connect")]
    public async Task<ActionResult<LockerResponse>> ConnectLocker([FromBody] ConnectLockerCommand command)
    {
        return await Mediator.Send(command);
    }
    
    [HttpGet("{id:int}/boxes")]
    public async Task<ActionResult<ListResponse<BoxStatus>>> GetAllBoxes([FromRoute] int id)
    {
        return await Mediator.Send(new GetAllBoxesQuery(id));
    }
    
    [HttpGet("{id:int}/timelines")]
    public async Task<ActionResult<PaginationResponse<LockerTimeline, LockerTimelineResponse>>> GetLockerTimeLines(
        [FromRoute] int id,
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
}