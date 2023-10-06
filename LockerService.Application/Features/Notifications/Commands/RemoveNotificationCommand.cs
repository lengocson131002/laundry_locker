using LockerService.Application.Features.Notifications.Models;

namespace LockerService.Application.Features.Notifications.Commands;

public record RemoveNotificationCommand(long Id) : IRequest<NotificationModel>;
