namespace LockerService.Application.Lockers.Queries;

public class GetAllBoxesQuery : IRequest<ListResponse<BoxStatus>>
{
    public long LockerId { get; private set; }

    public GetAllBoxesQuery(long lockerId)
    {
        this.LockerId = lockerId;
    }
}