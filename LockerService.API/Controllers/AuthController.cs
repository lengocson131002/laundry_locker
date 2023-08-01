using LockerService.Application.Auth.Queries;
using LockerService.Infrastructure.Common.Constants;
using LockerService.Infrastructure.Settings;

namespace LockerService.API.Controllers;

[ApiController]
[Route("/api/v1/auth")]
public class AuthController : ApiControllerBase
{
    private readonly IConfiguration _configuration;
    private readonly JwtSettings _jwtSettings;

    public AuthController(IConfiguration configuration, JwtSettings jwtSettings)
    {
        _configuration = configuration;
        _jwtSettings = jwtSettings;
    }

    [HttpPost("admin/login")]
    [AllowAnonymous]
    public async Task<ActionResult<TokenResponse>> LoginAdmin([FromBody] AdminLoginRequest request)
    {
        var response = await Mediator.Send(request);
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
        return response;
    }

    [HttpGet("staff/profile")]
    [Authorize(Roles = "Staff")]
    public async Task<ActionResult<AccountResponse>> GetStaffProfile()
    {
        return await Mediator.Send(new GetStaffProfileQuery());
    }
    
    [HttpPost("customer/login")]
    [AllowAnonymous]
    public async Task<ActionResult<TokenResponse>> LoginCustomer([FromBody] CustomerLoginRequest request)
    {
        var response = await Mediator.Send(request);
        return response;
    }

    [HttpPost("customer/verify")]
    [AllowAnonymous]
    public async Task<ActionResult<StatusResponse>> VerifyCustomer([FromBody] CustomerVerifyRequest request)
    {
        return await Mediator.Send(request);
    }

    [HttpGet("customer/profile")]
    [Authorize(Roles = "Customer")]
    public async Task<ActionResult<AccountResponse>> GetCustomerProfile()
    {
        return await Mediator.Send(new GetCustomerProfileQuery());
    }
    
    [HttpPost("refresh")]
    [AllowAnonymous]
    public async Task<ActionResult<TokenResponse>> RefreshToken([FromBody] RefreshTokenRequest request)
    {
        var response = await Mediator.Send(request); 
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
        var tokenExpireInMinutes = _jwtSettings.TokenExpire;
        var refreshTokenExpireInMinutes = _jwtSettings.RefreshTokenExpire;
        
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
                SameSite = SameSiteMode.None
            });

        return Task.CompletedTask;
    }

}