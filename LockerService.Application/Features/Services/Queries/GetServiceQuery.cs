using LockerService.Application.Features.Services.Models;

namespace LockerService.Application.Features.Services.Queries;

public record GetServiceQuery(long StoreId, long ServiceId) : IRequest<ServiceDetailResponse>;