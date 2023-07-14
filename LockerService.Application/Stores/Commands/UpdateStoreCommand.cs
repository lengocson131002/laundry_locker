namespace LockerService.Application.Stores.Commands;

public class UpdateStoreCommandValidator : AbstractValidator<UpdateStoreCommand>
{
    public UpdateStoreCommandValidator()
    {
        RuleFor(model => model.ContactPhone)
            .MaximumLength(20)
            .When(model => model is not null);

        RuleFor(model => model.Image)
            .MaximumLength(1000)
            .When(model => model is not null);
    }
}

public class UpdateStoreCommand : IRequest<StoreResponse>
{
    [JsonIgnore] 
    public long StoreId { get; set; }
    
    public string? ContactPhone { get; set; }
    
    public string? Image { get; set; }
}