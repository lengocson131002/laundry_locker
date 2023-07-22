namespace LockerService.Domain.Events;

public enum LockerEvent
{
    Connect,
    Disconnect,
    UpdateStatus,
    UpdateInformation,
    Overload,
}