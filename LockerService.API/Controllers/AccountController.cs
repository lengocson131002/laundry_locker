using LockerService.Domain.Enums;

namespace LockerService.API.Controllers;

[ApiController]
[Route("/api/v1/accounts")]
public class AccountController : ApiControllerBase
{
    [HttpPost("staffs")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<AccountResponse>> AddStaff([FromBody] AddStaffRequest request)
    {
        return await Mediator.Send(request);
    }
}