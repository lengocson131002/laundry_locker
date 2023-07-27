using LockerService.Application.Auth.Queries;
using LockerService.Application.Common.Extensions;
using LockerService.Infrastructure.Common.Constants;

namespace LockerService.API.Controllers;

[ApiController]
[Route("/api/v1/auth")]
public class AuthController : ApiControllerBase
{
    private readonly IConfiguration _configuration;

    public AuthController(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    [HttpPost("admin/login")]
    [AllowAnonymous]
    public async Task<ActionResult<TokenResponse>> LoginAdmin([FromBody] AdminLoginRequest request)
    {
        var response = await Mediator.Send(request);
        await SetHttpCookieToken(response);
        return response;
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
        var response = await Mediator.Send(request);
        await SetHttpCookieToken(response);
        return response;
    }

    [HttpPost("customer/login")]
    [AllowAnonymous]
    public async Task<ActionResult<TokenResponse>> LoginCustomer([FromBody] CustomerLoginRequest request)
    {
        var response = await Mediator.Send(request);
        await SetHttpCookieToken(response);
        return response;
    }

    [HttpPost("customer/verify")]
    [AllowAnonymous]
    public async Task<ActionResult<StatusResponse>> VerifyCustomer([FromBody] CustomerVerifyRequest request)
    {
        return await Mediator.Send(request);
    }

    [HttpPost("refresh")]
    [AllowAnonymous]
    public async Task<ActionResult<TokenResponse>> RefreshToken()
    {
        var refreshToken = Request.Cookies[TokenCookieConstants.RefreshTokenCookie];
        if (refreshToken == null)
        {
            return Unauthorized();
        }
        var request = new RefreshTokenRequest()
        {
            RefreshToken = refreshToken
        };
        var response = await Mediator.Send(request); 
        await SetHttpCookieToken(response);
        return response;
    }

    [HttpPut("password")]
    [Authorize]
    public async Task<ActionResult<StatusResponse>> ChangePassword([FromBody] UpdatePasswordCommand request)
    {
        return await Mediator.Send(request);
    }
    
    [HttpPost("logout")]
    public ActionResult Logout()
    {
        Response.Cookies.Delete(TokenCookieConstants.AccessTokenCookie);
        Response.Cookies.Delete(TokenCookieConstants.RefreshTokenCookie);
        return Ok();
    }

    private Task SetHttpCookieToken(TokenResponse token)
    {
        var tokenExpireInMinutes = _configuration.GetValueOrDefault("Jwt:TokenExpire", 5);
        var refreshTokenExpireInMinutes = _configuration.GetValueOrDefault("Jwt:RefreshTokenExpire", 30);
        
        HttpContext.Response.Cookies.Append(
            TokenCookieConstants.AccessTokenCookie, 
            token.AccessToken, 
            new CookieOptions()
            {
                Expires = DateTimeOffset.UtcNow.AddMinutes(tokenExpireInMinutes),
                HttpOnly = true,
                Secure = true,
                IsEssential = true,
                SameSite = SameSiteMode.None
            });
        
        HttpContext.Response.Cookies.Append(
            TokenCookieConstants.RefreshTokenCookie, 
            token.RefreshToken, 
            new CookieOptions()
            {
                Expires = DateTimeOffset.UtcNow.AddMinutes(refreshTokenExpireInMinutes),
                HttpOnly = true,
                Secure = true,
                IsEssential = true,
                SameSite = SameSiteMode.None
            });

        return Task.CompletedTask;
    }

}