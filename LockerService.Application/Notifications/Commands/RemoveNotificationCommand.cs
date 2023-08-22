using LockerService.Application.Notifications.Models;

namespace LockerService.Application.Notifications.Commands;

public record RemoveNotificationCommand(long Id) : IRequest<NotificationModel>;
