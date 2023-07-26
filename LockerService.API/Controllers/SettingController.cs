using Twilio.Rest.Insights.V1;

namespace LockerService.API.Controllers;

[ApiController]
[Route("/api/v1/settings")]
public class SettingController : ApiControllerBase
{
    [HttpGet]
    public async Task<ActionResult<ListResponse<SettingResource>>> GetAllSettings()
    {
        
    }
}