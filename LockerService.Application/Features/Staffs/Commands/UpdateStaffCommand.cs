using LockerService.Application.Features.Staffs.Models;
using LockerService.Shared.Extensions;

namespace LockerService.Application.Features.Staffs.Commands;

public class UpdateStaffCommandValidator : AbstractValidator<UpdateStaffCommand>
{
    public UpdateStaffCommandValidator()
    {
        RuleFor(model => model.PhoneNumber)
            .Must(phoneNumber => string.IsNullOrEmpty(phoneNumber) || phoneNumber.IsValidPhoneNumber())
            .WithMessage("Invalid Phone Number");

        RuleFor(model => model.Avatar)
            .Must(image => string.IsNullOrEmpty(image) || image.IsValidUrl())
            .WithMessage("Invalid image url");
        
        RuleFor(model => model.Role)
            .Must(role => role == null || Equals(Role.Manager, role) || Equals(Role.LaundryAttendant, role));
    }
}

public class UpdateStaffCommand : IRequest<StaffDetailResponse>
{
    [JsonIgnore] 
    public long Id { get; set; }
    
    [TrimString]
    public string? Username { get; set; }
    
    [TrimString(true)] 
    public string? FullName { get; set; }

    [NormalizePhone(true)]
    public string? PhoneNumber { get; set; }
    
    [TrimString] 
    public string? Avatar { get; set; }

    [TrimString] 
    public string? Description { get; set; }

    public long? StoreId { get; set; }
    
    public Role? Role { get; set; }
}