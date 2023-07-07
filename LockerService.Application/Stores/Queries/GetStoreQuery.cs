namespace LockerService.Application.Stores.Queries;

public class GetStoreQuery : IRequest<StoreDetailResponse>
{
    public int StoreId { get; init; }
}