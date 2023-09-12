namespace LockerService.Application.Orders.Commands;

public class AddLaundryItemCommandValidator : AbstractValidator<AddLaundryItemCommand>
{
    public AddLaundryItemCommandValidator()
    {
        RuleFor(model => model.Type)
            .NotNull();

        RuleFor(model => model.Image)
            .NotEmpty()
            .Must(image => image.IsValidUrl())
            .WithMessage("Invalid image url");

        RuleFor(model => model.Description)
            .MaximumLength(250)
            .When(model => model.Description != null);
    }
}
public class AddLaundryItemCommand : IRequest<LaundryItemResponse>
{
    [JsonIgnore]
    [BindNever]
    public long OrderId { get; set; }
    
    [JsonIgnore]
    [BindNever]
    public long OrderDetailId { get; set; }
    
    public ClothType? Type { get; set; }

    [TrimString(true)]
    public string Image { get; set; } = default!;
    
    [TrimString]
    public string? Description { get; set; }
}