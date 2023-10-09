using LockerService.API.Attributes;
using LockerService.API.Common;
using LockerService.Application.Features.Accounts.Models;
using LockerService.Application.Features.Auth.Commands;
using LockerService.Application.Features.Auth.Models;
using LockerService.Application.Features.Auth.Queries;
using LockerService.Domain.Enums;
using LockerService.Infrastructure.Common.Constants;
using LockerService.Infrastructure.Settings;

namespace LockerService.API.Controllers;

[ApiController]
[Route("/api/v1/auth")]
[ApiKey]
public class AuthController : ApiControllerBase
{
    private readonly JwtSettings _jwtSettings;

    public AuthController(JwtSettings jwtSettings)
    {
        _jwtSettings = jwtSettings;
    }

    /**
     * STAFF AUTH CONTROLLERS
     */
    [HttpPost("staff/login")]
    [AllowAnonymous]
    public async Task<ActionResult<AccessTokenResponse>> LoginStaff([FromBody] StaffLoginRequest request)
    {
        var response = await Mediator.Send(request);
        return response;
    }

    [HttpGet("staff/profile")]
    [AuthorizeRoles(Role.Admin, Role.Manager, Role.LaundryAttendant)]
    public async Task<ActionResult<AccountResponse>> GetStaffProfile()
    {
        return await Mediator.Send(new GetStaffProfileQuery());
    }
    
    [HttpPut("staff/profile")]
    [AuthorizeRoles(Role.Admin, Role.Manager, Role.LaundryAttendant)]
    public async Task<ActionResult<AccountResponse>> UpdateProfile([FromBody] UpdateStaffProfileCommand command)
    {
        return await Mediator.Send(command);
    }
    
    /**
     * CUSTOMER AUTH CONTROLLERS
     */
    [HttpPost("customer/login")]
    [AllowAnonymous]
    public async Task<ActionResult<AccessTokenResponse>> LoginCustomer([FromBody] CustomerLoginRequest request)
    {
        var response = await Mediator.Send(request);
        return response;
    }

    [HttpPost("customer/verify")]
    [AllowAnonymous]
    public async Task<ActionResult<OtpResponse>> VerifyCustomer([FromBody] CustomerVerifyRequest request)
    {
        return await Mediator.Send(request);
    }

    [HttpGet("customer/profile")]
    [AuthorizeRoles(Role.Customer)]
    public async Task<ActionResult<AccountResponse>> GetCustomerProfile()
    {
        return await Mediator.Send(new GetCustomerProfileQuery());
    }
    
    [HttpPut("customer/profile")]
    [AuthorizeRoles(Role.Customer)]
    public async Task<ActionResult<AccountResponse>> UpdateCustomerProfile([FromBody] UpdateCustomerProfileCommand command)
    {
        return await Mediator.Send(command);
    }

    [HttpPost("refresh")]
    [AllowAnonymous]
    public async Task<ActionResult<AccessTokenResponse>> RefreshToken([FromBody] RefreshTokenRequest request)
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

    [HttpPut("password/reset")]
    public async Task<ActionResult<StatusResponse>> ResetPassword([FromBody] ResetPasswordCommand request)
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

    private Task SetHttpCookieToken(AccessTokenResponse token)
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

    [HttpPost("device-token")]
    [Authorize]
    public async Task<ActionResult<TokenResponse>> RegisterDeviceToken([FromBody] RegisterDeviceTokenCommand command)
    {
        return await Mediator.Send(command);
    }

}