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
        [FromQuery] GetAllStaffsQuery query)
    {
        return await Mediator.Send(query);
    }

    [HttpGet("{id:long}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<StaffDetailResponse>> GetAllStaffs([FromRoute] long id)
    {
        var query = new GetStaffQuery
        {
            Id = id,
        };
        return await Mediator.Send(query);
    }

    [HttpPut("{id:long}/activate")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<StatusResponse>> ActivateStaff([FromRoute] long storeId,
        [FromRoute] long id)
    {
        var command = new ActivateStaffCommand()
        {
            Id = id,
            StoreId = storeId
        };
        return await Mediator.Send(command);
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
        [FromRoute] long storeId,
        [FromRoute] long id,
        UpdateStaffStatusCommand command)
    {
        command.Id = id;
        return await Mediator.Send(command);
    }

    [HttpDelete("{id:long}/revoke")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<StatusResponse>> RevokeStaff(
        [FromRoute] long id,
        RevokeStaffCommand command)
    {
        command.StaffId = id;
        return await Mediator.Send(command);
    }
}