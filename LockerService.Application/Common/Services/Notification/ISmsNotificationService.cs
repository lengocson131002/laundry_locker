using LockerService.Application.Common.Services.Notification.Data;

namespace LockerService.Application.Common.Services.Notification;

public interface ISmsNotificationService : INotificationService<SmsNotificationData>
{
    
}