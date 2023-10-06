using LockerService.Application.Common.Persistence.Repositories;
using LockerService.Application.Features.Lockers.Models;
using LockerService.Application.Features.Lockers.Queries;

namespace LockerService.Application.Features.Lockers.Handlers;

public class GetLockerEventStatisticHandler : IRequestHandler<LockerEventStatisticQuery, ListResponse<LockerEventStatisticItem>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetLockerEventStatisticHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ListResponse<LockerEventStatisticItem>> Handle(LockerEventStatisticQuery request, CancellationToken cancellationToken)
    {
        var lockerEvents = await _unitOfWork.LockerTimelineRepository
            .Get(@event => (request.From == null || @event.CreatedAt >= request.From)
                           && (request.To == null || @event.CreatedAt <= request.To)
                           && @event.LockerId == request.LockerId)
            .GroupBy(@event => @event.Event)
            .Select(item => new LockerEventStatisticItem()
            {
                Event = item.Key.Value,
                Count = item.Count()
            }).ToListAsync(cancellationToken);

        foreach (var @event in Enum.GetValues(typeof(LockerEvent)).Cast<LockerEvent>())
        {
            if (lockerEvents.All(item => !Equals(item.Event, @event)))
            {
                lockerEvents.Add(new LockerEventStatisticItem(@event));
            }
        }

        return new ListResponse<LockerEventStatisticItem>(lockerEvents);
    }
}