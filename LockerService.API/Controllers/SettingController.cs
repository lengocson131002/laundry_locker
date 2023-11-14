using LockerService.API.Attributes;
using LockerService.API.Common;
using LockerService.Application.Features.Settings.Commands;
using LockerService.Application.Features.Settings.Models;
using LockerService.Application.Features.Settings.Queries;
using LockerService.Domain.Enums;

namespace LockerService.API.Controllers;

/// <summary>
/// SETING API
/// </summary>
[ApiController]
[Route("/api/v1/settings")]
[ApiKey]
public class SettingController : ApiControllerBase
{
    /// <summary>
    /// Get all setting configurations
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    public async Task<ActionResult<SettingsResponse>> GetAllSettings()
    {
        return await Mediator.Send(new GetSettingsQuery());
    }

    /// <summary>
    /// Update setting values
    /// </summary>
    /// <param name="command"></param>
    /// <returns></returns>
    [HttpPut]
    [AuthorizeRoles(Role.Admin)]
    public async Task<ActionResult<SettingsResponse>> UpdateSettings([FromBody] UpdateSettingsCommand command)
    {
        return await Mediator.Send(command);
    }
}