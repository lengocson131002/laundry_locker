namespace LockerService.Application.Staffs.Commands;

public class UpdateStaffCommandValidator : AbstractValidator<UpdateStaffCommand>
{
    public UpdateStaffCommandValidator()
    {
        RuleFor(model => model.PhoneNumber)
            .Must(phoneNumber => phoneNumber == null || phoneNumber.IsValidPhoneNumber())
            .WithMessage("Invalid Phone Number");
    }
}

public class UpdateStaffCommand : IRequest<StaffDetailResponse>
{
    [JsonIgnore] public long Id { get; set; }

    public string? FullName { get; set; }

    public string? PhoneNumber { get; set; }

    public string? Avatar { get; set; }

    public string? Description { get; set; }

    public long? StoreId { get; set; }
}