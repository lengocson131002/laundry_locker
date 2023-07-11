using LockerService.Application.Lockers.Queries;

namespace LockerService.API.Controllers;

[ApiController]
[Route("/api/v1/auth")]
public class AuthController : ApiControllerBase
{
    [HttpPost("login/admin")]
    [AllowAnonymous]
    public async Task<ActionResult<TokenResponse>> LoginAdmin([FromBody] AdminLoginRequest request)
    {
        return await Mediator.Send(request);
    }

    [HttpPost("login/staff")]
    [AllowAnonymous]
    public async Task<ActionResult<TokenResponse>> LoginStaff([FromBody] AdminLoginRequest request)
    {
        return await Mediator.Send(request);
    }

    [HttpPost("refresh")]
    [AllowAnonymous]
    public async Task<ActionResult<TokenResponse>> RefreshToken([FromBody] RefreshTokenRequest request)
    {
        return await Mediator.Send(request);
    }

    [HttpGet("current")]
    [Authorize]
    public async Task<ActionResult<AccountResponse>> GetCurrentAccount()
    {
        return await Mediator.Send(new GetCurrentAccountQuery());
    }
}