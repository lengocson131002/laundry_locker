namespace LockerService.Application.Staffs.Commands;

public class ActivateStaffCommandValidator : AbstractValidator<ActivateStaffCommand>
{
}

public class ActivateStaffCommand : IRequest<StatusResponse>
{
    [JsonIgnore] 
    public long Id { get; set; } = default!;
    
    [JsonIgnore] 
    public long StoreId { get; set; } = default!;
}