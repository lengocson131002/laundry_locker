using Microsoft.AspNetCore.SignalR;

namespace LockerService.Infrastructure.SignalR.Notifications;

public class NotificationHub : Hub
{
    private readonly IConnectionManager _connectionManager;

    public NotificationHub(IConnectionManager connectionManager)
    {
        _connectionManager = connectionManager;
    }
    
    
}