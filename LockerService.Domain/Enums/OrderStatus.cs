namespace LockerService.Domain.Enums;

public enum OrderStatus
{
    Initialized,
    Waiting,
    Processing,
    Returned,
    Completed,
    Canceled
}