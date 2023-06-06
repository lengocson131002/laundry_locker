using LockerService.Domain.Events;

namespace LockerService.Application.Lockers.Models;

public class LockerTimelineResponse
{
    public LockerEvent? Event { get; set; }
    
    public LockerStatus? Status { get; set; }
    
    public LockerStatus? PreviousStatus { get; set; }
    
    public string? Description { get; set; }
    
    public int? ErrorCode { get; set; }
    
    public string? Error { get; set; }
    
    public DateTimeOffset Time { get; set; }
}