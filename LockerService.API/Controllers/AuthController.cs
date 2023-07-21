using LockerService.Application.Auth.Queries;
using LockerService.Application.Lockers.Queries;

namespace LockerService.API.Controllers;

[ApiController]
[Route("/api/v1/auth")]
public class AuthController : ApiControllerBase
{
    [HttpPost("admin/login")]
    [AllowAnonymous]
    public async Task<ActionResult<TokenResponse>> LoginAdmin([FromBody] AdminLoginRequest request)
    {
        return await Mediator.Send(request);
    }

    [HttpGet("admin/profile")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<AccountResponse>> GetAdminProfile()
    {
        return await Mediator.Send(new GetAdminProfileQuery());
    }

    [HttpPut("admin/profile")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<AccountResponse>> UpdateAdminProfile([FromBody] UpdateAdminProfileCommand command)
    {
        return await Mediator.Send(command);
    }

    [HttpPost("staff/login")]
    [AllowAnonymous]
    public async Task<ActionResult<TokenResponse>> LoginStaff([FromBody] StaffLoginRequest request)
    {
        return await Mediator.Send(request);
    }

    [HttpPost("customer/login")]
    [AllowAnonymous]
    public async Task<ActionResult<TokenResponse>> LoginCustomer([FromBody] CustomerLoginRequest request)
    {
        return await Mediator.Send(request);
    }

    [HttpPost("customer/verify")]
    [AllowAnonymous]
    public async Task<ActionResult<StatusResponse>> VerifyCustomer([FromBody] CustomerVerifyRequest request)
    {
        return await Mediator.Send(request);
    }

    [HttpPost("refresh")]
    [AllowAnonymous]
    public async Task<ActionResult<TokenResponse>> RefreshToken([FromBody] RefreshTokenRequest request)
    {
        return await Mediator.Send(request);
    }

    [HttpPut("password")]
    [Authorize]
    public async Task<ActionResult<StatusResponse>> ChangePassword([FromBody] UpdatePasswordCommand request)
    {
        return await Mediator.Send(request);
    }
}