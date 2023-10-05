using LockerService.Domain.Enums;

namespace LockerService.Infrastructure.Services.Notifications;

public class NotificationProvider : INotificationProvider
{
    private readonly Dictionary<NotificationType, IList<INotificationService>> _observers = new();
    
    public void Attach(NotificationType type, INotificationService notificationService)
    {
        if (!_observers.ContainsKey(type))
        {
            _observers[type] = new List<INotificationService>();
        }

        var services = _observers[type];
        if (services.All(service => service.GetType() != notificationService.GetType()))
        {
            services.Add(notificationService);
        }
    }

    public void Attach(ICollection<NotificationType> types, INotificationService notificationService)
    {
        foreach (var type in types)
        {
            Attach(type, notificationService);
        }
    }

    public void Attach(NotificationType type, ICollection<INotificationService> notificationServices)
    {
        foreach (var service in notificationServices)
        {
            Attach(type, service);
        }
    }

    public void Detach(NotificationType type, INotificationService notificationService)
    {
        if (_observers.ContainsKey(type))
        {
            _observers[type].Remove(notificationService);
        }
    }

    public async Task NotifyAsync(Notification notification)
    {
        if (_observers.TryGetValue(notification.Type, out var services))
        {
            foreach (var service in services)
            {
                await service.NotifyAsync(notification);
            }
        }
    }
}