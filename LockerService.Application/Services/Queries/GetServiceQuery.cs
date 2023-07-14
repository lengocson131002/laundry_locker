namespace LockerService.Application.Services.Queries;

public record GetServiceQuery(long ServiceId) : IRequest<ServiceDetailResponse>;