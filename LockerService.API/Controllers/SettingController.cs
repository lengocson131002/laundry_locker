using LockerService.API.Attributes;
using LockerService.API.Common;
using LockerService.Application.Settings.Commands;
using LockerService.Application.Settings.Models;
using LockerService.Application.Settings.Queries;
using LockerService.Domain.Enums;

namespace LockerService.API.Controllers;

[ApiController]
[Route("/api/v1/settings")]
[ApiKey]
public class SettingController : ApiControllerBase
{
    [HttpGet]
    public async Task<ActionResult<SettingsResponse>> GetAllSettings()
    {
        return await Mediator.Send(new GetSettingsQuery());
    }

    [HttpPut]
    [AuthorizeRoles(Role.Admin)]
    public async Task<ActionResult<SettingsResponse>> UpdateSettings([FromBody] UpdateSettingsCommand command)
    {
        return await Mediator.Send(command);
    }
}