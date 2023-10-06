using LockerService.Application.Features.Customers.Models;
using LockerService.Shared.Extensions;

namespace LockerService.Application.Features.Auth.Commands;

public class UpdateCustomerProfileCommandValidator : AbstractValidator<UpdateCustomerProfileCommand>
{
    public UpdateCustomerProfileCommandValidator()
    {
        RuleFor(model => model.Avatar)
            .Must(avatar => string.IsNullOrWhiteSpace(avatar) || avatar.IsValidUrl())
            .WithMessage("Invalid avatar url");
    }
}

public class UpdateCustomerProfileCommand : IRequest<CustomerResponse>
{
    [TrimString]
    public string? FullName { get; set; } = default!;

    [TrimString]
    public string? Avatar { get; set; } = default!;

    [TrimString]
    public string? Description { get; set; } = default!;

}