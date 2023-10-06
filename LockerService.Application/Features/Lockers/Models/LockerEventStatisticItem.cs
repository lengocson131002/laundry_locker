namespace LockerService.Application.Features.Lockers.Models;

public class LockerEventStatisticItem
{
    public LockerEvent Event { get; set; }
    
    public int Count { get; set; }

    public LockerEventStatisticItem()
    {
    }

    public LockerEventStatisticItem(LockerEvent @event)
    {
        Event = @event;
        Count = 0;
    }
    
}

