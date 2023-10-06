using LockerService.Application.Features.Lockers.Models;

namespace LockerService.Application.Features.Lockers.Queries;

public class LockerEventStatisticQuery : IRequest<ListResponse<LockerEventStatisticItem>>
{
    public DateTimeOffset? From { get; set; }
    
    public DateTimeOffset? To { get; set; }
    
    [BindNever]
    public long LockerId { get; set; }
}