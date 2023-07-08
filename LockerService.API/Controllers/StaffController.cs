namespace LockerService.API.Controllers;

[ApiController]
[Route("/api/v1/stores")]
public class StaffController : ApiControllerBase
{
    [HttpPost("{storeId}/staffs")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<AccountResponse>> AddStaff([FromRoute] int storeId, AddStaffCommand command)
    {
        command.StoreId = storeId;
        return await Mediator.Send(command);
    }

    [HttpGet("{storeId}/staffs")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<PaginationResponse<Account, AccountResponse>>> GetAllStaffs([FromRoute] int storeId,
        [FromQuery] GetAllStaffsQuery query)
    {
        query.StoreId = storeId;
        return await Mediator.Send(query);
    }

    [HttpGet("{storeId}/staffs/{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<AccountDetailResponse>> GetAllStaffs([FromRoute] int storeId,
        [FromRoute] int id)
    {
        var query = new GetStaffQuery
        {
            Id = id,
            StoreId = storeId
        };
        return await Mediator.Send(query);
    }

    [HttpPut("{storeId}/staffs/{id}/activate")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<StatusResponse>> ActivateStaff([FromRoute] int storeId,
        [FromRoute] int id)
    {
        var command = new ActivateStaffCommand()
        {
            Id = id,
            StoreId = storeId
        };
        return await Mediator.Send(command);
    }

    [HttpPut("{storeId}/staffs/{id}/deactivate")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<StatusResponse>> DeactivateStaff([FromRoute] int storeId,
        [FromRoute] int id)
    {
        var command = new DeactivateStaffCommand()
        {
            Id = id,
            StoreId = storeId
        };
        return await Mediator.Send(command);
    }

    [HttpDelete("{storeId}/staffs/{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<StatusResponse>> DeleteStaff([FromRoute] int storeId,
        [FromRoute] int id)
    {
        var command = new DeleteStaffCommand()
        {
            Id = id,
            StoreId = storeId
        };
        return await Mediator.Send(command);
    }
}