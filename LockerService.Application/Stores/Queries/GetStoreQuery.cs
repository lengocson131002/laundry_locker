namespace LockerService.Application.Stores.Queries;

public class GetStoreQuery : IRequest<StoreDetailResponse>
{
    public long StoreId { get; init; }
}