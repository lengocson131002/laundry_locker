namespace LockerService.Application.Features.Staffs.Commands;

public class AssignLockerCommandValidator : AbstractValidator<AssignLockerCommand>
{
    public AssignLockerCommandValidator()
    {
        RuleFor(model => model.LockerIds)
            .NotEmpty()
            .Must(UniqueStaffs)
            .WithMessage("LockerIds must contains unique ids");
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
public class AssignLockerCommand : IRequest<StatusResponse>
{
    [JsonIgnore]
    public long StaffId { get; set; }

    public IList<long> LockerIds { get; set; } = default!;
}