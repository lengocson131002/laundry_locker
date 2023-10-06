using LockerService.Application.Features.Locations.Commands;
using LockerService.Shared.Extensions;

namespace LockerService.Application.Features.Lockers.Commands;

public class UpdateLockerCommandValidator : AbstractValidator<UpdateLockerCommand>
{
    public UpdateLockerCommandValidator()
    {
        RuleFor(model => model.Name)
            .MaximumLength(100)
            .When(model => model.Name != null);
        
        RuleFor(model => model.Location)
            .SetInheritanceValidator(v => { v.Add(new AddLocationCommandValidator()); })
            .When(model => model.Location != null);
        
        RuleFor(model => model.Image)
            .Must(image => image == null || image.Trim().IsValidUrl())
            .WithMessage("Invalid image url");

        RuleFor(model => model.OrderTypes)
            .Must(orderTypes => orderTypes == null ||
                                (orderTypes.Count > 0 && orderTypes.Distinct().Count() == orderTypes.Count()))
            .WithMessage("OrderType must not empty and contains unique values");
    }
    
    private bool UniqueStaffs(IList<long> staffIds)
    {
        var encounteredIds = new HashSet<long>();

        foreach (var element in staffIds)
        {
            if (!encounteredIds.Contains(element))
            {
                encounteredIds.Add(element);
            }
            else
            {
                return false;
            }
        }
        return true;
    }
}

public class UpdateLockerCommand : IRequest
{
    [JsonIgnore] 
    public long LockerId { get; set; }
    
    [TrimString(true)]
    public string? Name { get; set; }

    [TrimString]
    public string? Image { get; set; }
    
    [TrimString]
    public string? Description { get; set; }

    public LocationCommand? Location { get; set; }
    
    public long? StoreId { get; set; }
    
    public IList<OrderType>? OrderTypes { get; set; }
}