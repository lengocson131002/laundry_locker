namespace LockerService.Application.Lockers.Commands;

public class UpdateLockerCommandValidator : AbstractValidator<UpdateLockerCommand>
{
    public UpdateLockerCommandValidator()
    {
        RuleFor(model => model.StaffIds)
            .NotEmpty()
            .When(model => model.StaffIds != null);
        
        RuleFor(model => model.StaffIds)
            .Must(staffIds => staffIds == null || (staffIds.Any() && UniqueStaffs(staffIds)))
            .WithMessage("StaffIds must not be empty and must contains unique ids");
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
    
    public IList<long>? StaffIds { get; set; }
}