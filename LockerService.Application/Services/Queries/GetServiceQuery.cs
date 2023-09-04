namespace LockerService.Application.Services.Queries;

public record GetServiceQuery(long StoreId, long ServiceId) : IRequest<ServiceDetailResponse>;