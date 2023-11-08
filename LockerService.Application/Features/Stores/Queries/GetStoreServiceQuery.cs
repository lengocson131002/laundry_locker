using LockerService.Application.Features.Services.Models;

namespace LockerService.Application.Features.Stores.Queries;

public record GetStoreServiceQuery (long StoreId, long ServiceId) : IRequest<ServiceDetailResponse>;