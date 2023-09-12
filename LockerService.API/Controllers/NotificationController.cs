using LockerService.API.Attributes;
using LockerService.Application.Common.Enums;
using LockerService.Application.Notifications.Commands;
using LockerService.Application.Notifications.Models;
using LockerService.Application.Notifications.Queries;

namespace LockerService.API.Controllers;

[ApiController]
[Route("/api/v1/notifications")]
[ApiKey]
public class NotificationController : ApiControllerBase
{
    [HttpPost("push")]
    public async Task<ActionResult> PushNotification(PushNotificationCommand command)
    {
        await Mediator.Send(command);
        return Ok();
    }

    [HttpGet]
    [Authorize]
    public async Task<ActionResult<PaginationResponse<Notification, NotificationModel>>> GetAllNotifications([FromQuery] GetAllNotificationsQuery query)
    {
        if (string.IsNullOrWhiteSpace(query.SortColumn))
        {
            query.SortColumn = "CreatedAt";
            query.SortDir = SortDirection.Desc;
        }
        return await Mediator.Send(query);
    }

    [HttpGet("{id:long}")]
    [Authorize]
    public async Task<ActionResult<NotificationModel>> GetNotificationDetail(long id)
    {
        var query = new GetNotificationQuery(id);
        return await Mediator.Send(query);
    }

    [HttpPut("{id:long}")]
    [Authorize]
    public async Task<ActionResult<NotificationModel>> UpdateNotificationStatus(long id,
        [FromBody] UpdateNotificationStatusCommand command)
    {
        command.Id = id;
        return await Mediator.Send(command);
    }

    [HttpDelete("{id:long}")]
    [Authorize]
    public async Task<ActionResult<NotificationModel>> RemoveNotification(long id)
    {
        var command = new RemoveNotificationCommand(id);
        return await Mediator.Send(command);
    }

    [HttpGet("unread-count")]
    [Authorize]
    public async Task<ActionResult<UnreadNotificationCountResponse>> CountUnread([FromQuery] GetUnreadNotificationCountQuery query)
    {
        return await Mediator.Send(query);
    }
}