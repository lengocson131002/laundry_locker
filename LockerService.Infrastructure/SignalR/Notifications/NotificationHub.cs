using Microsoft.AspNetCore.SignalR;

namespace LockerService.Infrastructure.SignalR.Notifications;

public class NotificationHub : Hub
{
    private readonly IConnectionManager _connectionManager;
    private readonly IUnitOfWork _unitOfWork;
    public NotificationHub(ConnectionManagerServiceResolver resolver, IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
        _connectionManager = resolver(typeof(NotificationConnectionManager));
    }

    public async Task<string> Connect()
    {
        var httpContext = Context.GetHttpContext();
        var accId = long.Parse(httpContext.Request.Query["accountId"]);

        var account = await _unitOfWork.AccountRepository.GetByIdAsync(accId);
        if (account == null)
        {
            throw new Exception("Account not found");
        }
        
        _connectionManager.KeepConnection(accId, Context.ConnectionId);
        return Context.ConnectionId;
    }


    public override Task OnDisconnectedAsync(Exception? exception)
    {
        var connectionId = Context.ConnectionId;
        _connectionManager.RemoveConnection(connectionId);
        return Task.CompletedTask;
    }
}