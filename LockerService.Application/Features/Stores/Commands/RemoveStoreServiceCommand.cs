namespace LockerService.Application.Features.Stores.Commands;

public record RemoveStoreServiceCommand(long StoreId, long ServiceId) : IRequest<StatusResponse>;
