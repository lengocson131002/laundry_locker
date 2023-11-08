using LockerService.Application.Features.Services.Models;

namespace LockerService.Application.Features.Services.Queries;

public class GetServiceQuery: IRequest<ServiceDetailResponse>
{
    public long StoreId { get; set; }
    
    public long ServiceId { get; set; }

    public GetServiceQuery(long storeId, long serviceId)
    {
        StoreId = storeId;
        ServiceId = serviceId;
    }

    public GetServiceQuery(long serviceId)
    {
        ServiceId = serviceId;
    }
}