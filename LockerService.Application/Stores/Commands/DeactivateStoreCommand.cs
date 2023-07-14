namespace LockerService.Application.Stores.Commands;

public class DeactivateStoreCommandValidator : AbstractValidator<DeactivateStoreCommand>
{
}

public class DeactivateStoreCommand : IRequest<StoreResponse>
{
    [JsonIgnore] public int StoreId { get; set; }
}