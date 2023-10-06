using LockerService.Application.Features.Stores.Models;

namespace LockerService.Application.Features.Stores.Commands;

public class UpdateStoreStatusCommandValidator : AbstractValidator<UpdateStoreStatusCommand>
{
    public UpdateStoreStatusCommandValidator()
    {
        RuleFor(model => model.Status)
            .NotNull();
    }
}

public class UpdateStoreStatusCommand : IRequest<StoreResponse>
{
    [JsonIgnore]
    public long StoreId { get; set; }

    public StoreStatus Status { get; set; }
}