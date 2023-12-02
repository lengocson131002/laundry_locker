using LockerService.Application.Common.Persistence.Repositories;
using LockerService.Domain.Enums;
using LockerService.Shared.Utils;
using Microsoft.Extensions.DependencyInjection;
using INotificationService = LockerService.Application.Common.Services.Notifications.INotificationService;

namespace LockerService.Infrastructure.Services.Notifications;

public class Notifier : INotifier
{
    private readonly INotificationProvider _provider;
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly ILogger<Notifier> _logger;
    
    public Notifier(
        INotificationProvider provider,
        IWebNotificationService webNotificationService,
        IMobileNotificationService mobileNotificationService,
        ISmsNotificationService smsNotificationService, 
        IServiceScopeFactory serviceScopeFactory, ILogger<Notifier> logger)
    {
        _provider = provider;
        _serviceScopeFactory = serviceScopeFactory;
        _logger = logger;

        // COMMON NOTIFICATION TYPE
        _provider.Attach(NotificationType.AccountOtpCreated, new List<INotificationService> ()
        {
            smsNotificationService
        }) ;
        
        
        // SYSTEM NOTIFICATION TYPE
        _provider.Attach(NotificationType.SystemStaffCreated, new List<INotificationService>()
        {
            smsNotificationService,
        });
        
        _provider.Attach(NotificationType.SystemLockerConnected, new List<INotificationService>()
        {
            webNotificationService,
        });
        
        _provider.Attach(NotificationType.SystemLockerDisconnected, new List<INotificationService>()
        {
            webNotificationService,
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

        _provider.Attach(NotificationType.SystemOrderOverTime, new List<INotificationService>()
        {
            webNotificationService,
            mobileNotificationService
        });
        
        
        // CUSTOMER NOTIFICATION TYPE
        _provider.Attach(NotificationType.CustomerOrderCreated, new List<INotificationService>()
        {
            mobileNotificationService,
            smsNotificationService
        });
        
        _provider.Attach(NotificationType.CustomerOrderReturned, new List<INotificationService>()
        {
            mobileNotificationService,
            smsNotificationService
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
            smsNotificationService
        });
        
        _provider.Attach(NotificationType.CustomerOrderOverTimeProcessing, new List<INotificationService>()
        {
            mobileNotificationService,
        });
        
        _provider.Attach(NotificationType.CustomerDepositCompleted, new List<INotificationService>()
        {
            mobileNotificationService,
        });
    }
    
    public async Task NotifyAsync(Notification notification)
    {
        if (notification.Saved)
        {
            // persist notification into database
            using var scope = _serviceScopeFactory.CreateScope();
            var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
            await unitOfWork.NotificationRepository.AddAsync(notification);
            await unitOfWork.SaveChangesAsync();
        }
        
        _logger.LogInformation("[PUSH NOTIFICATION]: {0}", JsonSerializerUtils.Serialize(notification));
        await _provider.NotifyAsync(notification);
    }
}