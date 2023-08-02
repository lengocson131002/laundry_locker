namespace LockerService.Infrastructure.SignalR;

public interface IConnectionManager
{
    void KeepConnection(long accId, string connectionId);
    
    void RemoveConnection(string connectionId);

    List<string> GetConnections(long accId);
}