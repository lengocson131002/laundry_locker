using LockerService.API.Attributes;
using LockerService.Application.Common.Enums;
using LockerService.Application.Features.Notifications.Commands;
using LockerService.Application.Features.Notifications.Models;
using LockerService.Application.Features.Notifications.Queries;

namespace LockerService.API.Controllers;

/// <summary>
/// NOTIFICATION API
/// </summary>
[ApiController]
[Route("/api/v1/notifications")]
[ApiKey]
public class NotificationController : ApiControllerBase
{
    /// <summary>
    /// [TEST] Push notification
    /// </summary>
    /// <param name="command"></param>
    /// <returns></returns>
    [HttpPost("push")]
    public async Task<ActionResult<NotificationModel>> PushNotification(PushNotificationCommand command)
    {
        return await Mediator.Send(command);
    }

    /// <summary>
    /// Get all notifications
    /// </summary>
    /// <param name="query"></param>
    /// <returns></returns>
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

    /// <summary>
    /// Get a notification detail
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpGet("{id:long}")]
    [Authorize]
    public async Task<ActionResult<NotificationModel>> GetNotificationDetail(long id)
    {
        var query = new GetNotificationQuery(id);
        return await Mediator.Send(query);
    }

    /// <summary>
    /// Update a notification's status (read / unread)
    /// </summary>
    /// <param name="id"></param>
    /// <param name="command"></param>
    /// <returns></returns>
    [HttpPut("{id:long}")]
    [Authorize]
    public async Task<ActionResult<NotificationModel>> UpdateNotificationStatus(long id,
        [FromBody] UpdateNotificationStatusCommand command)
    {
        command.Id = id;
        return await Mediator.Send(command);
    }

    /// <summary>
    /// Remove a notification
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpDelete("{id:long}")]
    [Authorize]
    public async Task<ActionResult<NotificationModel>> RemoveNotification(long id)
    {
        var command = new RemoveNotificationCommand(id);
        return await Mediator.Send(command);
    }

    /// <summary>
    /// Count unread notification
    /// </summary>
    /// <param name="query"></param>
    /// <returns></returns>
    [HttpGet("unread-count")]
    [Authorize]
    public async Task<ActionResult<UnreadNotificationCountResponse>> CountUnread([FromQuery] GetUnreadNotificationCountQuery query)
    {
        return await Mediator.Send(query);
    }
}