namespace LockerService.Application.Stores.Commands;

public class ActivateStoreCommandValidator : AbstractValidator<ActivateStoreCommand>
{
}

public class ActivateStoreCommand : IRequest<StoreResponse>
{
    [JsonIgnore] 
    public long StoreId { get; set; }
}