namespace LockerService.Application.Services.Queries;

public record GetServiceQuery(int ServiceId) : IRequest<ServiceDetailResponse>;