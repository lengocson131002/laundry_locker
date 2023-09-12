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

        // COMMON NOTIFICATION TYPE
        _provider.Attach(NotificationType.AccountOtpCreated, new List<INotificationService> ()
        {
            smsNotificationService
        }) ;
        
        
        // SYSTEM NOTIFICATION TYPE
        _provider.Attach(NotificationType.SystemStaffCreated, new List<INotificationService>()
        {
            // smsNotificationService,
            mobileNotificationService,
        });
        
        _provider.Attach(NotificationType.SystemLockerConnected, new List<INotificationService>()
        {
            webNotificationService,
            mobileNotificationService,
        });
        
        _provider.Attach(NotificationType.SystemLockerDisconnected, new List<INotificationService>()
        {
            webNotificationService,
            mobileNotificationService,
        });
        
        _provider.Attach(NotificationType.SystemLockerBoxOverloaded, new List<INotificationService>()
        {
            webNotificationService,
            mobileNotificationService
        });
        
        _provider.Attach(NotificationType.SystemLockerBoxWarning, new List<INotificationService>()
        {
            webNotificationService,
            mobileNotificationService
        });
        
        _provider.Attach(NotificationType.SystemOrderCreated, new List<INotificationService>()
        {
            webNotificationService,
            mobileNotificationService
        });
        
        _provider.Attach(NotificationType.SystemOrderCollected, new List<INotificationService>()
        {
            webNotificationService,
            mobileNotificationService
        });
        
        _provider.Attach(NotificationType.SystemOrderProcessed, new List<INotificationService>()
        {
            webNotificationService,
            mobileNotificationService
        });
        
        _provider.Attach(NotificationType.SystemOrderOverTime, new List<INotificationService>()
        {
            webNotificationService,
            mobileNotificationService
        });
        
        
        // CUSTOMER NOTIFICATION TYPE
        _provider.Attach(NotificationType.CustomerOrderCreated, new List<INotificationService>()
        {
            mobileNotificationService,
            // smsNotificationService
        });
        
        _provider.Attach(NotificationType.CustomerOrderReturned, new List<INotificationService>()
        {
            mobileNotificationService,
            // smsNotificationService
        });
        
        _provider.Attach(NotificationType.CustomerOrderCanceled, new List<INotificationService>()
        {
            mobileNotificationService,
        });
        
        _provider.Attach(NotificationType.CustomerOrderCompleted, new List<INotificationService>()
        {
            mobileNotificationService,
        });
        
        _provider.Attach(NotificationType.CustomerOrderOverTime, new List<INotificationService>()
        {
            mobileNotificationService,
            // smsNotificationService
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