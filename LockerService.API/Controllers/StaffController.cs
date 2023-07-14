using LockerService.Application.Staffs.Models;

namespace LockerService.API.Controllers;

[ApiController]
[Route("/api/v1/stores")]
public class StaffController : ApiControllerBase
{
    [HttpPost("{storeId:long}/staffs")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<StaffResponse>> AddStaff([FromRoute] long storeId, AddStaffCommand command)
    {
        command.StoreId = storeId;
        return await Mediator.Send(command);
    }

    [HttpGet("{storeId:long}/staffs")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<PaginationResponse<Account, StaffResponse>>> GetAllStaffs([FromRoute] long storeId, [FromQuery] GetAllStaffsQuery query)
    {
        query.StoreId = storeId;
        return await Mediator.Send(query);
    }

    [HttpGet("{storeId:long}/staffs/{id:long}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<StaffDetailResponse>> GetAllStaffs([FromRoute] long storeId,
        [FromRoute] long id)
    {
        var query = new GetStaffQuery
        {
            Id = id,
            StoreId = storeId
        };
        return await Mediator.Send(query);
    }

    [HttpPut("{storeId:long}/staffs/{id:long}/activate")]
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

    [HttpPut("{storeId:long}/staffs/{id:long}/deactivate")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<StatusResponse>> DeactivateStaff([FromRoute] long storeId,
        [FromRoute] long id)
    {
        var command = new DeactivateStaffCommand()
        {
            Id = id,
            StoreId = storeId
        };
        return await Mediator.Send(command);
    }

    [HttpDelete("{storeId:long}/staffs/{id:long}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<StatusResponse>> DeleteStaff([FromRoute] long storeId,
        [FromRoute] long id)
    {
        var command = new DeleteStaffCommand()
        {
            Id = id,
            StoreId = storeId
        };
        return await Mediator.Send(command);
    }

    [HttpPost("{storeId:long}/staffs/{id:long}/assign")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<StatusResponse>> AssignStaff(
        [FromRoute] long storeId,
        [FromRoute] long id, 
        AssignStaffCommand command)
    {
        command.Id = id;
        command.StoreId = storeId;
        return await Mediator.Send(command);
    }
    
    [HttpDelete("{storeId:long}/staffs/{id:long}/revoke")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<StatusResponse>> RevokeStaff(
        [FromRoute] long storeId,
        [FromRoute] long id, 
        RevokeStaffCommand command)
    {
        command.Id = id;
        command.StoreId = storeId;
        return await Mediator.Send(command);
    }
}