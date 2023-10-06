using LockerService.Application.Features.Locations.Commands;
using LockerService.Application.Features.Stores.Models;
using LockerService.Shared.Extensions;

namespace LockerService.Application.Features.Stores.Commands;

public class UpdateStoreCommandValidator : AbstractValidator<UpdateStoreCommand>
{
    public UpdateStoreCommandValidator()
    {
        RuleFor(model => model.Name)
            .MaximumLength(100)
            .When(model => model.Name is not null);

        RuleFor(model => model.Location)
            .SetInheritanceValidator(v => { v.Add(new AddLocationCommandValidator()); })
            .When(model => model.Location is not null);

        RuleFor(model => model.ContactPhone)
            .Must(contactPhone => contactPhone == null || contactPhone.IsValidPhoneNumber())
            .WithMessage("Invalid Contact Phone");

        RuleFor(model => model.Image)
            .MaximumLength(1000)
            .Must(image => image.IsValidUrl())
            .When(model => !string.IsNullOrWhiteSpace(model.Image))
            .WithMessage("Invalid image URL");
    }
}

public class UpdateStoreCommand : IRequest<StoreResponse>
{
    [JsonIgnore] 
    public long StoreId { get; set; }

    [TrimString(true)]
    public string? Name { get; set; }
    
    [NormalizePhone(true)]
    public string? ContactPhone { get; set; }

    public LocationCommand? Location { get; set; }

    [TrimString]
    public string? Image { get; set; }
    
    [TrimString]
    public string? Description { get; set; }
}