using LockerService.Application.Common.Services.Notifications;
using LockerService.Domain.Enums;
using Microsoft.Extensions.DependencyInjection;
using INotificationService = LockerService.Application.Common.Services.Notifications.INotificationService;

namespace LockerService.Infrastructure.Services.Notifications;

public class Notifier : INotifier
{
    private readonly INotificationProvider _provider;
    private readonly IServiceScopeFactory _serviceScopeFactory;
    
    public Notifier(
        INotificationProvider provider,
        IWebNotificationService webNotificationService,
        IMobileNotificationService mobileNotificationService,
        ISmsNotificationService smsNotificationService, 
        IServiceScopeFactory serviceScopeFactory)
    {
        _provider = provider;
        _serviceScopeFactory = serviceScopeFactory;

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
         * _provider.Attach(NotificationType.TypeC, smsNotificationService);
         *
         */
        
        // Account notifications
        _provider.Attach(NotificationType.AccountOtpCreated, new List<INotificationService>()
        {
            smsNotificationService
        });
        
        _provider.Attach(NotificationType.AccountStaffCreated, new List<INotificationService>()
        {
            smsNotificationService
        });
        
        
        // Order notifications
        _provider.Attach(NotificationType.OrderCreated, new List<INotificationService>()
        {
            mobileNotificationService,
            smsNotificationService
        });
        
        _provider.Attach(NotificationType.OrderReturned, new List<INotificationService>()
        {
            mobileNotificationService,
            smsNotificationService
        });
        
        _provider.Attach(NotificationType.OrderCanceled, new List<INotificationService>()
        {
            mobileNotificationService,
            smsNotificationService
        });
        
        _provider.Attach(NotificationType.OrderCompleted, new List<INotificationService>()
        {
            mobileNotificationService    
        });
    }
    
    public async Task NotifyAsync(Notification notification)
    {
        await _provider.NotifyAsync(notification);

        if (notification.Saved)
        {
            // persist notification into database
            using var scope = _serviceScopeFactory.CreateScope();
            var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
            await unitOfWork.NotificationRepository.AddAsync(notification);
            await unitOfWork.SaveChangesAsync();
        }
    }
}