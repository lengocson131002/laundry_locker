namespace LockerService.Application.Customers.Queries;

public record GetCustomerDetailQuery(long Id) : IRequest<CustomerDetailResponse>;