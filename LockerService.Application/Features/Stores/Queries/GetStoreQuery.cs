using LockerService.Application.Features.Stores.Models;

namespace LockerService.Application.Features.Stores.Queries;

public class GetStoreQuery : IRequest<StoreDetailResponse>
{
    public long StoreId { get; init; }
}