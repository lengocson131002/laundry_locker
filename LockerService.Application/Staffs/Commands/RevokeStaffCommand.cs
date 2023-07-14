namespace LockerService.Application.Staffs.Commands;

public class RevokeStaffCommandValidator : AbstractValidator<RevokeStaffCommand>
{
    public RevokeStaffCommandValidator()
    {
        RuleFor(model => model.LockerId)
            .NotNull();
    }
}

public class RevokeStaffCommand : IRequest<StatusResponse>
{
    [JsonIgnore] 
    public long Id { get; set; } = default!;
    
    [JsonIgnore] 
    public long StoreId { get; set; } = default!;
    
    public long LockerId { get; set; } = default!;
}