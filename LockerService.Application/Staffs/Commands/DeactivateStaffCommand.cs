namespace LockerService.Application.Staffs.Commands;

public class DeactivateStaffCommandValidator : AbstractValidator<DeactivateStaffCommand>
{
    public DeactivateStaffCommandValidator()
    {
    }
}

public class DeactivateStaffCommand : IRequest<StatusResponse>
{
    [JsonIgnore] 
    
    public long Id { get; set; } = default!;
    
    [JsonIgnore] 
    public long StoreId { get; set; } = default!;
}