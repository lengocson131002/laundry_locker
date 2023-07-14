using LockerService.Domain.Events;

namespace LockerService.Application.Lockers.Queries;

public class GetLockerTimelinesQuery : PaginationRequest<LockerTimeline>, IRequest<PaginationResponse<LockerTimeline, LockerTimelineResponse>>
{
    [BindNever]
    public long? LockerId { get; set; }
    
    public LockerEvent? Event { get; set; }

    public DateTimeOffset? From { get; set; }
    
    public DateTimeOffset? To { get; set; }

    public override Expression<Func<LockerTimeline, bool>> GetExpressions()
    {
        Expression = Expression.And(lockerTimeline => LockerId == null || lockerTimeline.LockerId == LockerId);

        Expression = Expression.And(lockerTimeline => Event == null || Event.Equals(lockerTimeline.Event));

        Expression = Expression.And(lockerTimeline => From == null || lockerTimeline.CreatedAt.UtcDateTime >= From);

        Expression = Expression.And(lockerTimeline => To == null || lockerTimeline.CreatedAt.UtcDateTime <= To);

        return Expression;
    }
}