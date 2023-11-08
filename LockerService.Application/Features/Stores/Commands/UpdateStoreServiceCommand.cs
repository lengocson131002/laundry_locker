namespace LockerService.Application.Features.Stores.Commands;

public class UpdateStoreServiceCommandValidator : AbstractValidator<UpdateStoreServiceCommand>
{
    public UpdateStoreServiceCommandValidator()
    {
        RuleFor(model => model.Price)
            .GreaterThan(0);
    }
}

public class UpdateStoreServiceCommand : IRequest<StatusResponse>
{
    [JsonIgnore]
    public long StoreId { get; set; }
    
    [JsonIgnore]
    public long ServiceId { get; set; }
    
    public decimal Price { get; set; }

    public UpdateStoreServiceCommand(long storeId, long serviceId, decimal price)
    {
        StoreId = storeId;
        ServiceId = serviceId;
        Price = price;
    }
}