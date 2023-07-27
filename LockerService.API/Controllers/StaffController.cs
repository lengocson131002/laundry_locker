using LockerService.Application.Common.Enums;
using LockerService.Application.Staffs.Models;

namespace LockerService.API.Controllers;

[ApiController]
[Route("/api/v1/staffs")]
public class StaffController : ApiControllerBase
{
    [HttpPost("")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<StaffDetailResponse>> AddStaff(AddStaffCommand command)
    {
        return await Mediator.Send(command);
    }

    [HttpGet("")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<PaginationResponse<Account, StaffResponse>>> GetAllStaffs(
        [FromQuery] GetAllStaffsQuery request)
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
    public async Task<ActionResult<StaffDetailResponse>> GetStaff([FromRoute] long id)
    {
        var query = new GetStaffQuery
        {
            Id = id,
        };
        return await Mediator.Send(query);
    }

    [HttpPut("{id:long}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<StaffDetailResponse>> UpdateStaff(
        [FromRoute] long id,
        UpdateStaffCommand command)
    {
        command.Id = id;
        return await Mediator.Send(command);
    }

    [HttpPut("{id:long}/status")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<StaffDetailResponse>> UpdateStaffStatus(
        [FromRoute] long id,
        UpdateStaffStatusCommand command)
    {
        command.Id = id;
        return await Mediator.Send(command);
    }

    [HttpPost("{id:long}/lockers")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<StatusResponse>> AssignLocker([FromRoute] long id, [FromBody] AssignLockerCommand command)
    {
        command.StaffId = id;
        return await Mediator.Send(command);
    }
    
    [HttpDelete("{id:long}/lockers")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<StatusResponse>> RevokeLocker([FromRoute] long id, [FromBody] RevokeLockerCommand command)
    {
        command.StaffId = id;
        return await Mediator.Send(command);
    }
}