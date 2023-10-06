using LockerService.Application.Features.Customers.Models;

namespace LockerService.Application.Features.Customers.Queries;

public record GetCustomerDetailQuery(long Id) : IRequest<CustomerDetailResponse>;