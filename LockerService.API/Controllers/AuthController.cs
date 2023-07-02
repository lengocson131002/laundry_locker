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
    [AllowAnonymous]
    public async Task<ActionResult<TokenResponse>> RefreshToken([FromBody] RefreshTokenRequest request)
    {
        return await Mediator.Send(request);
    }
}