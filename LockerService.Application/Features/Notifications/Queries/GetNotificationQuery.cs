using LockerService.Application.Features.Notifications.Models;

namespace LockerService.Application.Features.Notifications.Queries;

public record GetNotificationQuery(long Id) : IRequest<NotificationModel>;
