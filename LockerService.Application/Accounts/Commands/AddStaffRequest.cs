namespace LockerService.Application.Accounts.Commands;

public class AddStaffRequestValidator : AbstractValidator<AddStaffRequest>
{
    public AddStaffRequestValidator()
    {
        RuleFor(model => model.PhoneNumber)
            .NotEmpty();
    }
}

public class AddStaffRequest : IRequest<AccountResponse>
{
    public string PhoneNumber { get; set; } = default!;
    public int? StoreId { get; set; }
}