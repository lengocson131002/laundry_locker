using LockerService.Application.Features.Lockers.Models;

namespace LockerService.Application.Features.Lockers.Queries;

public class GetLockerQuery : IRequest<LockerDetailResponse>
{
    public long LockerId { get; init; }
}