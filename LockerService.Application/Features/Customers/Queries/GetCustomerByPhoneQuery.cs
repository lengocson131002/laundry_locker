using LockerService.Application.Features.Customers.Models;

namespace LockerService.Application.Features.Customers.Queries;

public record GetCustomerByPhoneQuery(string Phone): IRequest<CustomerDetailResponse>;