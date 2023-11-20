using LockerService.API.Attributes;
using LockerService.API.Common;
using LockerService.Application.Features.Accounts.Models;
using LockerService.Application.Features.Auth.Commands;
using LockerService.Application.Features.Auth.Models;
using LockerService.Application.Features.Auth.Queries;
using LockerService.Application.Features.Tokens.Models;
using LockerService.Domain.Enums;

namespace LockerService.API.Controllers;

/// <summary>
/// AUTH API
/// </summary>
[ApiController]
[Route("/api/v1/auth")]
[ApiKey]
public class AuthController : ApiControllerBase
{

    
    /// <summary>
    /// Staff login by username & password
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPost("staff/login")]
    [AllowAnonymous]
    public async Task<ActionResult<AccessTokenResponse>> LoginStaff([FromBody] StaffLoginRequest request)
    {
        var response = await Mediator.Send(request);
        return response;
    }

    /// <summary>
    /// Get staff's profile
    /// </summary>
    /// <returns></returns>
    [HttpGet("staff/profile")]
    [AuthorizeRoles(Role.Admin, Role.Manager, Role.LaundryAttendant)]
    public async Task<ActionResult<AccountResponse>> GetStaffProfile()
    {
        return await Mediator.Send(new GetStaffProfileQuery());
    }
    
    /// <summary>
    /// Update staff's profile
    /// </summary>
    /// <param name="command"></param>
    /// <returns></returns>
    [HttpPut("staff/profile")]
    [AuthorizeRoles(Role.Admin, Role.Manager, Role.LaundryAttendant)]
    public async Task<ActionResult<AccountResponse>> UpdateProfile([FromBody] UpdateStaffProfileCommand command)
    {
        return await Mediator.Send(command);
    }
    

    
    /// <summary>
    /// Customer login by phone number & otp
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPost("customer/login")]
    [AllowAnonymous]
    public async Task<ActionResult<AccessTokenResponse>> LoginCustomer([FromBody] CustomerLoginRequest request)
    {
        var response = await Mediator.Send(request);
        return response;
    }

    /// <summary>
    /// Get Customer OTP by phone number
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPost("customer/verify")]
    [AllowAnonymous]
    public async Task<ActionResult<OtpResponse>> VerifyCustomer([FromBody] CustomerVerifyRequest request)
    {
        return await Mediator.Send(request);
    }

    /// <summary>
    /// Get customer's profile
    /// </summary>
    /// <returns></returns>
    [HttpGet("customer/profile")]
    [AuthorizeRoles(Role.Customer)]
    public async Task<ActionResult<AccountResponse>> GetCustomerProfile()
    {
        return await Mediator.Send(new GetCustomerProfileQuery());
    }
    
    /// <summary>
    /// Update customer profile
    /// </summary>
    /// <param name="command"></param>
    /// <returns></returns>
    [HttpPut("customer/profile")]
    [AuthorizeRoles(Role.Customer)]
    public async Task<ActionResult<AccountResponse>> UpdateCustomerProfile([FromBody] UpdateCustomerProfileCommand command)
    {
        return await Mediator.Send(command);
    }

    /// <summary>
    /// Refresh token
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPost("refresh")]
    [AllowAnonymous]
    public async Task<ActionResult<AccessTokenResponse>> RefreshToken([FromBody] RefreshTokenRequest request)
    {
        var response = await Mediator.Send(request); 
        return response;
    }

    /// <summary>
    /// Update staff's password
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPut("password")]
    [Authorize]
    public async Task<ActionResult<StatusResponse>> ChangePassword([FromBody] UpdatePasswordCommand request)
    {
        return await Mediator.Send(request);
    }

    /// <summary>
    /// Reset staff's password
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPut("password/reset")]
    public async Task<ActionResult<StatusResponse>> ResetPassword([FromBody] ResetPasswordCommand request)
    {
        return await Mediator.Send(request);
    }

    /// <summary>
    /// [MOBILE APP] Register a device token when user has just login into mobile application
    /// </summary>
    /// <param name="command"></param>
    /// <returns></returns>
    [HttpPost("device-token")]
    [Authorize]
    public async Task<ActionResult<TokenResponse>> RegisterDeviceToken([FromBody] RegisterDeviceTokenCommand command)
    {
        return await Mediator.Send(command);
    }

    /// <summary>
    /// Logout API. If mobile app, pass device token to remove current device token from system to prevent receive notifications after logout
    /// </summary>
    /// <param name="command"></param>
    /// <returns></returns>
    [Authorize]
    [HttpPost("logout")]
    public async Task<ActionResult<StatusResponse>> Logout([FromBody] LogoutCommand command)
    {
        return await Mediator.Send(command);
    }

}