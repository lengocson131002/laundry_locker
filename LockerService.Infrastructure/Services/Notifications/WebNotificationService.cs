using LockerService.Application.Common.Services.Notifications;

namespace LockerService.Infrastructure.Services.Notifications;

public class WebNotificationService : IWebNotificationService
{
    private readonly ILogger<WebNotificationService> _logger;

    public WebNotificationService(ILogger<WebNotificationService> logger)
    {
        _logger = logger;
    }

    public Task NotifyAsync(Notification notification)
    {        
        _logger.LogInformation("Handle web notification: {0}", JsonSerializer.Serialize(notification));
        return Task.CompletedTask;
    }
}