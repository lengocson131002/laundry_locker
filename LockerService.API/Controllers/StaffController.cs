using LockerService.API.Attributes;
using LockerService.Application.Staffs.Models;

namespace LockerService.API.Controllers;

[ApiController]
[Route("/api/v1/staffs")]
[Authorize]
[ApiKey]
public class StaffController : ApiControllerBase
{
    [HttpPost("")]
    public async Task<ActionResult<StaffDetailResponse>> AddStaff(AddStaffCommand command)
    {
        return await Mediator.Send(command);
    }

    [HttpGet("")]
    public async Task<ActionResult<PaginationResponse<Account, StaffResponse>>> GetAllStaffs(
        [FromQuery] GetAllStaffsQuery request)
    {
        return await Mediator.Send(request);
    }

    [HttpGet("{id:long}")]
    public async Task<ActionResult<StaffDetailResponse>> GetStaff([FromRoute] long id)
    {
        var query = new GetStaffQuery
        {
            Id = id,
        };
        return await Mediator.Send(query);
    }

    [HttpPut("{id:long}")]
    public async Task<ActionResult<StaffDetailResponse>> UpdateStaff(
        [FromRoute] long id,
        UpdateStaffCommand command)
    {
        command.Id = id;
        return await Mediator.Send(command);
    }

    [HttpPut("{id:long}/status")]
    public async Task<ActionResult<StaffDetailResponse>> UpdateStaffStatus(
        [FromRoute] long id,
        UpdateStaffStatusCommand command)
    {
        command.Id = id;
        return await Mediator.Send(command);
    }
}