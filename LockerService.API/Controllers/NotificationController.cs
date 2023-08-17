using LockerService.Application.Notifications.Commands;

namespace LockerService.API.Controllers;

[ApiController]
[Route("/api/v1/notifications")]
public class NotificationController : ApiControllerBase
{
    [HttpPost("push")]
    public async Task<ActionResult> PushNotification(PushNotificationCommand command)
    {
        await Mediator.Send(command);
        return Ok();
    }
}