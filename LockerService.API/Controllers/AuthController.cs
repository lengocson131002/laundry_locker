using LockerService.Application.Common.Extensions;
using LockerService.Domain.Enums;

namespace LockerService.API.Controllers;

[ApiController]
[Route("/api/v1/auth")]
public class AuthController : ApiControllerBase
{
    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<ActionResult<TokenResponse>> Login([FromBody] LoginRequest request)
    {
        return await Mediator.Send(request);
    }
    
    [HttpPost("refresh")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<TokenResponse>> RefreshToken()
    {
        return await Mediator.Send(new RefreshTokenRequest());
    }
}