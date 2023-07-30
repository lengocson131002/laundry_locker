using LockerService.Application.Common.Services.Notifications;

namespace LockerService.Infrastructure.Services.Notifications;

public class FirebaseNotificationService : IMobileNotificationService
{
    private readonly ILogger<FirebaseNotificationService> _logger;

    public FirebaseNotificationService(ILogger<FirebaseNotificationService> logger)
    {
        _logger = logger;
    }

    public Task NotifyAsync(Notification notification)
    {
        _logger.LogInformation("Handle firebase notification: {0}", JsonSerializer.Serialize(notification));
        return Task.CompletedTask;
    }
}