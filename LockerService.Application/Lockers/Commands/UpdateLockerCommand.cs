using LockerService.Application.Common.Utils;

namespace LockerService.Application.Lockers.Commands;

public class UpdateLockerCommandValidator : AbstractValidator<UpdateLockerCommand>
{
    public UpdateLockerCommandValidator()
    {
        RuleFor(model => model.StaffIds)
            .NotEmpty()
            .When(model => model.StaffIds != null);
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
    
    public IList<long>? StaffIds { get; set; }
}