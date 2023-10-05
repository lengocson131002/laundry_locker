using AutoMapper;
using LockerService.Application.Notifications.Models;
using LockerService.Infrastructure.SignalR;
using LockerService.Infrastructure.SignalR.Notifications;
using Microsoft.AspNetCore.SignalR;

namespace LockerService.Infrastructure.Services.Notifications;

public class WebNotificationService : IWebNotificationService
{
    private readonly ILogger<WebNotificationService> _logger;
    private readonly IHubContext<NotificationHub> _notificationHubContext;
    private readonly IConnectionManager _connectionManager;
    private readonly IMapper _mapper;
    private const string ReceiveNotificationFunctionName = "ReceiveNotification";

    public WebNotificationService(
        ILogger<WebNotificationService> logger, 
        IMapper mapper, 
        IHubContext<NotificationHub> notificationHubContext,
        ConnectionManagerServiceResolver connectionManagerServiceResolver)
    {
        _logger = logger;
        _mapper = mapper;
        _notificationHubContext = notificationHubContext;
        _connectionManager = connectionManagerServiceResolver(typeof(NotificationConnectionManager));
    }

    public async Task NotifyAsync(Notification notification)
    {
        var notificationModel = _mapper.Map<NotificationModel>(notification);
        
        var connections = _connectionManager.GetConnections(notification.AccountId);
        if (connections.Any())
        {
            foreach (var connection in connections)
            {
                await _notificationHubContext.Clients.Client(connection).SendAsync(ReceiveNotificationFunctionName, notificationModel);
            }
        }
        
        _logger.LogInformation($"[WEB NOTIFICATION] Send notification: {0}", notification.Id);
    }
}