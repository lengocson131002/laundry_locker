using LockerService.Application.Common.Services.Notification;
using LockerService.Application.Common.Services.Notification.Data;

namespace LockerService.Infrastructure.Services.Notification;

public class WebNotificationService : IWebNotificationService
{
    public Task SendAsync(WebNotificationData data)
    {
        throw new NotImplementedException();
    }
}