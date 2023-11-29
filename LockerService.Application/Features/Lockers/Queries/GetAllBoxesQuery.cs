using LockerService.Application.Features.Lockers.Models;

namespace LockerService.Application.Features.Lockers.Queries;

public class GetAllBoxesQuery : IRequest<ListResponse<BoxResponse>>
{
    public long LockerId { get; private set; }
    
    public bool? IsActive { get; set; }

    public GetAllBoxesQuery(long lockerId)
    {
        this.LockerId = lockerId;
    }
}