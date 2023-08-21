using LockerService.Application.Common.Services.Notifications;
using LockerService.Domain.Enums;
using Microsoft.Extensions.DependencyInjection;

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
        
        _provider.Attach(NotificationType.AccountOtpCreated, smsNotificationService);
    }
    
    public async Task NotifyAsync(Notification notification)
    {
        await _provider.NotifyAsync(notification);
        
        // persist notification into database
        using var scope = _serviceScopeFactory.CreateScope();
        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
        await unitOfWork.NotificationRepository.AddAsync(notification);
        await unitOfWork.SaveChangesAsync();
    }
}