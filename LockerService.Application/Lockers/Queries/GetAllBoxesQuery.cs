namespace LockerService.Application.Lockers.Queries;

public class GetAllBoxesQuery : IRequest<ListResponse<BoxStatus>>
{
    public int LockerId { get; private set; }

    public GetAllBoxesQuery(int lockerId)
    {
        this.LockerId = lockerId;
    }
}