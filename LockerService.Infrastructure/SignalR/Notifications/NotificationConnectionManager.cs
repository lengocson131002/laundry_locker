namespace LockerService.Infrastructure.SignalR.Notifications;

public class NotificationConnectionManager : IConnectionManager
{
    private static Dictionary<long, List<string>> _connections = new(); 
        
    public void KeepConnection(long accId, string connectionId)
    {
        lock (_connections)
        {
            if (!_connections.ContainsKey(accId))
            {
                _connections.Add(accId, new());
            }
            _connections[accId].Add(connectionId);
        }
    }

    public void RemoveConnection(string connectionId)
    {
        lock (_connections)
        {
            foreach (var accId in _connections.Keys)
            {
                if (_connections[accId].Contains(connectionId))
                {
                    _connections[accId].Remove(connectionId);
                    break;
                }
            }
        }
    }

    public List<string> GetConnections(long accId)
    {
        List<string> connections;
        lock (_connections)
        {
            connections = _connections[accId];
        }
        return connections;
    }
}