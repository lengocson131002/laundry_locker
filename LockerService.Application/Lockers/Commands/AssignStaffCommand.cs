namespace LockerService.Application.Lockers.Commands;

public class AssignStaffCommandValidator : AbstractValidator<AssignStaffCommand>
{
    public AssignStaffCommandValidator()
    {
        RuleFor(model => model.StaffIds)
            .NotNull()
            .Must(UniqueStaffs)
            .WithMessage("StaffIds must contains unique ids");
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

public class AssignStaffCommand : IRequest<StatusResponse>
{
    [JsonIgnore] 
    public long LockerId { get; set; }

    public IList<long> StaffIds { get; set; } = default!;
}