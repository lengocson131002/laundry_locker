namespace LockerService.Application.Staffs.Commands;

public class RevokeLockerCommandValidator : AbstractValidator<RevokeLockerCommand>
{
    public RevokeLockerCommandValidator()
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
public class RevokeLockerCommand : IRequest<StatusResponse>
{
    [JsonIgnore]
    public long StaffId { get; set; }

    public IList<long> LockerIds { get; set; } = default!;
}