using LockerService.Application.Notifications.Models;

namespace LockerService.Application.Notifications.Queries;

public record GetNotificationQuery(long Id) : IRequest<NotificationModel>;
