using LockerService.Application.Common.Services.Notifications;

namespace LockerService.Infrastructure.Services.Notifications;

public class Notifier : INotifier
{
    private readonly INotificationProvider _provider;
    
    public Notifier(
        INotificationProvider provider,
        IWebNotificationService webNotificationService,
        IMobileNotificationService mobileNotificationService,
        ISmsNotificationService smsNotificationService)
    {
        _provider = provider;
        
        /*
         * Subscribe notification type and corresponding handle service
         *
         * _provider.Attach(NotificationType.TypeA, webNotificationService);
         * _provider.Attach(NotificationType.TypeA, mobileNotificationService);
         *
         * _provider.Attach(NotificationType.TypeB, webNotificationService);
         * _provider.Attach(NotificationType.TypeB, smsNotificationService);
         *
         * _provider.Attach(NotificationType.TypeC, webNotificationService);
         * _provider.Attach(NotificationType.TypeC, mobileNotificationService);
         *_provider.Attach(NotificationType.TypeC, smsNotificationService);
         *
         */
    }
    
    public async Task NotifyAsync(Notification notification)
    {
        await _provider.NotifyAsync(notification);
    }
}