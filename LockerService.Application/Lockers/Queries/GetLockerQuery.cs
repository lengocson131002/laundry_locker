namespace LockerService.Application.Lockers.Queries;

public class GetLockerQuery : IRequest<LockerDetailResponse>
{
    public long LockerId { get; init; }
}