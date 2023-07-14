namespace LockerService.Application.Staffs.Commands;

public class DeleteStaffCommandValidator : AbstractValidator<DeleteStaffCommand>
{
    public DeleteStaffCommandValidator()
    {
    }
}

public class DeleteStaffCommand : IRequest<StatusResponse>
{
    [JsonIgnore] 
    public long Id { get; set; } = default!;
    
    [JsonIgnore] 
    public long StoreId { get; set; } = default!;
}