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
    
    [TrimString(true)]
    public string? Name { get; set; }
    
    [TrimString(true)]
    public string? Image { get; set; }

    public decimal? Price { get; set; }
    
    [TrimString]
    public string? Unit { get; set; }
    
    [TrimString]
    public string? Description { get; set; }
    
    public ServiceStatus? Status { get; set; }
}