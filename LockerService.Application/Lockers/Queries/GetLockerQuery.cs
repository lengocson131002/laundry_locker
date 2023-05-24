namespace LockerService.Application.Lockers.Queries;

public class GetLockerQuery : IRequest<LockerDetailResponse>
{
    public int LockerId { get; init; }
}