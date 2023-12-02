using LockerService.Application.Features.Customers.Models;
using LockerService.Shared.Extensions;

namespace LockerService.Application.Features.Customers.Queries;

public class GetCustomerByPhoneQueryValidator : AbstractValidator<GetCustomerByPhoneQuery>
{
    public GetCustomerByPhoneQueryValidator()
    {
        RuleFor(model => model.Phone)
            .NotEmpty()
            .Must(phone => phone.IsValidPhoneNumber())
            .WithMessage("Phone number is not valid");
            
    }
}

public class GetCustomerByPhoneQuery : IRequest<CustomerDetailResponse>
{
    [NormalizePhone(true)]
    public string Phone { get; set; }

    public GetCustomerByPhoneQuery(string phone)
    {
        Phone = phone;
    }
}