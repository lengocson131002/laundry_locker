namespace LockerService.Application.Customers.Queries;

public record GetCustomerByPhoneQuery(string Phone): IRequest<CustomerDetailResponse>;