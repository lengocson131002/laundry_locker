using LockerService.Application.Settings.Commands;
using LockerService.Application.Settings.Models;
using LockerService.Application.Settings.Queries;
using Twilio.Rest.Insights.V1;

namespace LockerService.API.Controllers;

[ApiController]
[Route("/api/v1/settings")]
public class SettingController : ApiControllerBase
{
    [HttpGet]
    public async Task<ActionResult<SettingsResponse>> GetAllSettings()
    {
        return await Mediator.Send(new GetSettingsQuery());
    }

    [HttpPut]
    public async Task<ActionResult<SettingsResponse>> UpdateSettings([FromBody] UpdateSettingsCommand command)
    {
        return await Mediator.Send(command);
    }
}