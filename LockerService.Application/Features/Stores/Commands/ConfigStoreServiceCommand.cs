using LockerService.Shared.Utils;

namespace LockerService.Application.Features.Stores.Commands;

public class ConfigStoreServiceCommandValidator : AbstractValidator<ConfigStoreServiceCommand>
{
    public ConfigStoreServiceCommandValidator()
    {
        RuleFor(model => model.Services)
            .NotEmpty();

        RuleFor(model => model.Services)
            .Must(services => services.ContainsUniqueElements(item => new { item.ServiceId }))
            .WithMessage("Must contains unique service ids");

        RuleForEach(model => model.Services)
            .SetValidator(new StoreServiceItemValidator());
    }
}

public class ConfigStoreServiceCommand : IRequest<StatusResponse>
{
    [JsonIgnore]
    public  long StoreId { get; set; }
    
    public List<StoreServiceItem> Services { get; set; } = new List<StoreServiceItem>();
}


public class StoreServiceItemValidator : AbstractValidator<StoreServiceItem> 
{
    public StoreServiceItemValidator()
    {
        RuleFor(model => model.ServiceId)
            .GreaterThan(0);

        RuleFor(model => model.Price)
            .GreaterThan(0)
            .When(model => model.Price != null);
    }
}
public class StoreServiceItem
{
    public long ServiceId { get; set; }
    
    public decimal? Price { get; set; }
    
}