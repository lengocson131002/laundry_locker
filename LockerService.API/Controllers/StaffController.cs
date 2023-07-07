using LockerService.Application.Staffs.Queries;
using LockerService.Domain.Enums;

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
        GetAllStaffsQuery query)
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
}