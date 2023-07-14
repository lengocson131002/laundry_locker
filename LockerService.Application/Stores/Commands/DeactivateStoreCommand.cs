namespace LockerService.Application.Stores.Commands;

public class DeactivateStoreCommandValidator : AbstractValidator<DeactivateStoreCommand>
{
}

public class DeactivateStoreCommand : IRequest<StoreResponse>
{
    [JsonIgnore] 
    public long StoreId { get; set; }
}