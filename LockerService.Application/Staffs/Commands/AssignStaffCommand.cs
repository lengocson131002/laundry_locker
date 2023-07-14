namespace LockerService.Application.Staffs.Commands;

public class AssignStaffCommandValidator : AbstractValidator<AssignStaffCommand>
{
    public AssignStaffCommandValidator()
    {
        RuleFor(model => model.LockerId)
            .NotNull();
    }
}

public class AssignStaffCommand : IRequest<StatusResponse>
{
    [JsonIgnore] 
    public long Id { get; set; } = default!;
    
    [JsonIgnore] 
    public long StoreId { get; set; } = default!;
    
    public long LockerId { get; set; } = default!;
}