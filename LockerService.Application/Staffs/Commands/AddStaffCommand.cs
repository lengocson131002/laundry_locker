namespace LockerService.Application.Staffs.Commands;

public class AddStaffCommandValidator : AbstractValidator<AddStaffCommand>
{
    public AddStaffCommandValidator()
    {
        RuleFor(model => model.PhoneNumber)
            .NotEmpty();
    }
}

public class AddStaffCommand : IRequest<AccountResponse>
{
    public string PhoneNumber { get; set; } = default!;
    [JsonIgnore] public int StoreId { get; set; } = default!;
}